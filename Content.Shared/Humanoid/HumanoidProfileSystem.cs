using Content.Shared._Erida.Language.Components;
using Content.Shared._Erida.Language.Systems;
using Content.Shared._Erida.Preference;
using Content.Shared._Erida.TTS;
using Content.Shared.Examine;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.IdentityManagement;
using Content.Shared.Preferences;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid;

public sealed class HumanoidProfileSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly GrammarSystem _grammar = default!;

    // Corvax-TTS-Start
    public const string DefaultVoice = "Aidar";

    public static readonly Dictionary<Sex, string> DefaultSexVoice = new()
    {
        { Sex.Male, "Aidar" },
        { Sex.Female, "Kseniya" },
        { Sex.Unsexed, "Baya" },
    };
    // Corvax-TTS-End

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HumanoidProfileComponent, ExaminedEvent>(OnExamined);
    }

    public void ApplyProfileTo(Entity<HumanoidProfileComponent?> ent, HumanoidCharacterProfile profile)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        ent.Comp.Gender = profile.Gender;
        ent.Comp.Age = profile.Age;
        ent.Comp.Species = profile.Species;
        ent.Comp.Sex = profile.Sex;
        SetTTSVoice(ent.Owner, profile.Voice, ent.Comp);
        SetCorporation(ent.Owner, profile.Corporation, ent.Comp);
        Dirty(ent);

        var sexChanged = new SexChangedEvent(ent.Comp.Sex, profile.Sex);
        RaiseLocalEvent(ent, ref sexChanged);

        if (TryComp<GrammarComponent>(ent, out var grammar))
        {
            _grammar.SetGender((ent, grammar), profile.Gender);
        }
    }

    private void OnExamined(Entity<HumanoidProfileComponent> ent, ref ExaminedEvent args)
    {
        var identity = Identity.Entity(ent, EntityManager);
        var species = GetSpeciesRepresentation(ent.Comp.Species).ToLower();
        var age = GetAgeRepresentation(ent.Comp.Species, ent.Comp.Age);

        args.PushText(Loc.GetString("humanoid-appearance-component-examine", ("user", identity), ("age", age), ("species", species)));
    }

    // Corvax-TTS-Start
    // ReSharper disable once InconsistentNaming
    public void SetTTSVoice(EntityUid uid, string voiceId, HumanoidProfileComponent humanoid)
    {
        if (!TryComp<TTSComponent>(uid, out var comp))
            return;

        humanoid.Voice = voiceId;
        comp.VoicePrototypeId = voiceId;
    }
    // Corvax-TTS-End

    // Erida start
    public void SetCorporation(EntityUid uid, CorporationPreference corporation, HumanoidProfileComponent humanoid)
    {
        humanoid.Corporation = corporation;

        if (!TryComp<LanguageKnowledgeComponent>(uid, out var comp))
            return;

        if (corporation == CorporationPreference.NanoTrasen || corporation == CorporationPreference.Syndicate) {
            var languageName = "Codespeak" + $"{corporation}";

            comp.SpokenLanguages.Add(languageName);
            comp.UnderstoodLanguages.Add(languageName);

            RaiseLocalEvent(uid, new LanguageListUpdated());
        }
    }
    // Erida end


    /// <summary>
    /// Takes ID of the species prototype, returns UI-friendly name of the species.
    /// </summary>
    public string GetSpeciesRepresentation(ProtoId<SpeciesPrototype> species)
    {
        if (_prototype.TryIndex(species, out var speciesPrototype))
            return Loc.GetString(speciesPrototype.Name);

        Log.Error("Tried to get representation of unknown species: {speciesId}");
        return Loc.GetString("humanoid-appearance-component-unknown-species");
    }

    /// <summary>
    /// Takes ID of the species prototype and an age, returns an approximate description
    /// </summary>
    public string GetAgeRepresentation(ProtoId<SpeciesPrototype> species, int age)
    {
        if (!_prototype.TryIndex(species, out var speciesPrototype))
        {
            Log.Error("Tried to get age representation of species that couldn't be indexed: " + species);
            return Loc.GetString("identity-age-young");
        }

        if (age < speciesPrototype.YoungAge)
        {
            return Loc.GetString("identity-age-young");
        }

        if (age < speciesPrototype.OldAge)
        {
            return Loc.GetString("identity-age-middle-aged");
        }

        return Loc.GetString("identity-age-old");
    }
}
