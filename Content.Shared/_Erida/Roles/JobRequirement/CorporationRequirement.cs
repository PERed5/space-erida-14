using System.Diagnostics.CodeAnalysis;
using System.Text;
using Content.Shared._Erida.Preference;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._Erida.Roles;

[UsedImplicitly]
[Serializable, NetSerializable]
public sealed partial class CorporationRequirement : JobRequirement
{
    [DataField(required: true)]
    public HashSet<CorporationPreference> Corporations = new();

    public override bool Check(IEntityManager entManager,
        IPrototypeManager protoManager,
        HumanoidCharacterProfile? profile,
        IReadOnlyDictionary<string, TimeSpan> playTimes,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = new FormattedMessage();

        if (profile is null)
            return true;

        var sb = new StringBuilder();
        sb.Append("[color=yellow]");
        foreach (var s in Corporations)
        {
            sb.Append(Loc.GetString($"humanoid-profile-editor-preference-corporation-{s.ToString().ToLower()}"));
        }

        sb.Append("[/color]");

        if (!Inverted)
        {
            reason = FormattedMessage.FromMarkupPermissive($"{Loc.GetString("role-timer-whitelisted-corporation")} {sb}");

            if (!Corporations.Contains(profile.Corporation))
                return false;
        }
        else
        {
            reason = FormattedMessage.FromMarkupPermissive($"{Loc.GetString("role-timer-blacklisted-corporation")} {sb}");

            if (Corporations.Contains(profile.Corporation))
                return false;
        }

        return true;
    }
}
