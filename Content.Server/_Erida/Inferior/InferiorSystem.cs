using Content.Shared.IdentityManagement;
using Content.Shared.Implants;
using Content.Shared.Mindshield.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared._Erida.Inferior;
using Content.Server.Store.Systems;
using Content.Shared._Erida.Inferior.Components;
using Content.Shared.Implants.Components;
using Content.Shared.NPC.Systems;
using Robust.Shared.Prototypes;
using Content.Shared.NPC.Prototypes;
using Content.Server.Roles;
using Content.Server.Mind;
using Content.Server.Antag;
using System.Diagnostics;
using Robust.Shared.Player;
using Content.Shared._Erida.Roles.Components;
using Content.Shared.Mind.Components;
using Robust.Shared.Placement;
using Robust.Shared.Containers;
using Content.Server._Erida.Inferior.Components;
using Content.Server.Administration.Logs;
using Content.Shared.Database;

namespace Content.Server._Erida.Inferior;

public sealed class InferiorSystem : SharedInferiorSystem
{
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedStunSystem _sharedStun = default!;
    [Dependency] private readonly NpcFactionSystem _npcFaction = default!;
    [Dependency] private readonly RoleSystem _role = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly ISharedPlayerManager _player = default!;
    [Dependency] private readonly IAdminLogManager _adminLogManager = default!;
    public ProtoId<NpcFactionPrototype> NanoTrasenFaction = "NanoTrasen";
    public ProtoId<NpcFactionPrototype> SyndicateFaction = "Syndicate";
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InferiorImplantComponent, EntGotInsertedIntoContainerMessage>(OnInsert);
        SubscribeLocalEvent<InferiorImplantComponent, EntGotRemovedFromContainerMessage>(OnRemove);
        SubscribeLocalEvent<InferiorImplantComponent, ImplantImplantedEvent>(OnImplantImplanted);

        SubscribeLocalEvent<InferiorComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<InferiorComponent, ComponentShutdown>(OnShutdown);

        //SubscribeLocalEvent<InferiorImplantComponent, ImplantRemovedEvent>(OnImplantRemoved);
        SubscribeLocalEvent<StoreBuyFinishedEvent>(StoreBuyFinishedEvent);
        SubscribeLocalEvent<InferiorRoleComponent, GetBriefingEvent>(OnGetBriefing);
        SubscribeLocalEvent<InferiorComponent, MindAddedMessage>(OnMindAdded);
        SubscribeLocalEvent<InferiorComponent, MindRemovedMessage>(OnMindRemoved);
    }

    private void OnInsert(EntityUid uid, InferiorImplantComponent component, EntGotInsertedIntoContainerMessage args)
    {
        if (args.Container.ID == "implanter_slot")
        {
            component.ImplanterUid = args.Container.Owner;
        }
    }
    private void OnRemove(EntityUid uid, InferiorImplantComponent component, EntGotRemovedFromContainerMessage args)
    {
        if (args.Container.ID == "implant")
        {
            if (HasComp<InferiorComponent>(args.Container.Owner))
                RemComp<InferiorComponent>(args.Container.Owner);
        }
    }
    private void OnImplantImplanted(Entity<InferiorImplantComponent> entity, ref ImplantImplantedEvent ev)
    {
        EnsureComp<InferiorComponent>(ev.Implanted, out var comp);

        if (entity.Comp.Overlord != null)
            comp.Overlord = entity.Comp.Overlord;
        else if (entity.Comp.ImplanterUid != null)
            comp.Overlord = Transform(entity.Comp.ImplanterUid.Value).ParentUid;
        else
            return;

        entity.Comp.ImplanterUid = null;
        Start(ev.Implanted, comp);
    }

    public void Start(EntityUid uid, InferiorComponent comp)
    {
        if (HasComp<MindShieldComponent>(uid)
            || !_mindSystem.TryGetMind(uid, out var mindId, out var mind)
            || uid == comp.Overlord
            || comp.Overlord == null)
        {
            return;
        }

        _npcFaction.RemoveFaction(uid, NanoTrasenFaction, false);
        _npcFaction.AddFaction(uid, SyndicateFaction);

        _role.MindAddRole(mindId, "MindRoleInferior");
        if (_role.MindHasRole<InferiorRoleComponent>(mindId, out var infRole))
        {
            infRole.Value.Comp2.Overlord = comp.Overlord.Value;
            Dirty(infRole.Value.Owner, infRole.Value.Comp2);
        }
        else
        {
            return;
        }

        if (mind is { UserId: not null } && _player.TryGetSessionById(mind.UserId, out var session))
            _antag.SendBriefing(session, Loc.GetString("inferior-role-greeting", ("overlord", infRole.Value.Comp2.Overlord)), null, comp.InfStartSound);
    }
    private void OnMindAdded(EntityUid uid, InferiorComponent component, MindAddedMessage args)
    {
        if (!_role.MindHasRole<InferiorRoleComponent>(args.Mind.Owner))
        {
            Start(uid, component);
        }
    }
    private void OnMindRemoved(EntityUid uid, InferiorComponent component, MindRemovedMessage args)
    {
        _role.MindRemoveRole<InferiorRoleComponent>(args.Mind.Owner);
    }
    private void OnGetBriefing(EntityUid uid, InferiorRoleComponent comp, ref GetBriefingEvent args)
    {
        args.Append(Loc.GetString("inferior-briefing", ("overlord", comp.Overlord)));
    }
    public void OnStartup(EntityUid uid, InferiorComponent component, ComponentStartup arg)
    {
        _sharedStun.TryUpdateParalyzeDuration(uid, TimeSpan.FromSeconds(5f)); //dont need this but, but its a still a good indicator from how Revulution and subverted silicone does it
    }
    public void OnShutdown(EntityUid uid, InferiorComponent component, ComponentShutdown arg)
    {
        _sharedStun.TryUpdateParalyzeDuration(uid, TimeSpan.FromSeconds(5f));
        if (_mindSystem.TryGetMind(uid, out var mindId, out _))
            _role.MindRemoveRole<InferiorRoleComponent>(mindId);
        _popupSystem.PopupEntity(Loc.GetString("inferior-popup-stop"), uid, PopupType.Large);
        _adminLogManager.Add(LogType.Mind, LogImpact.Medium, $"{ToPrettyString(uid)} is no longer Inferior.");
    }
    private void StoreBuyFinishedEvent(ref StoreBuyFinishedEvent ev)
    {
        if (ev.PurchasedItemEntity != null
            && TryComp<ImplanterComponent>(ev.PurchasedItemEntity.Value, out var compImp)
            && compImp.ImplanterSlot.ContainerSlot != null
            && compImp.ImplanterSlot.ContainerSlot.ContainedEntity != null
            && TryComp<InferiorImplantComponent>(compImp.ImplanterSlot.ContainerSlot.ContainedEntity, out var compInf))
        {
            compInf.Overlord = ev.User;
        }
    }
}
