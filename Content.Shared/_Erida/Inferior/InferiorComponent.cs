using Content.Shared._Erida.Inferior;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Erida.Inferior.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedInferiorSystem))]
public sealed partial class InferiorComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public EntityUid? Overlord = null;
    [DataField]
    public SoundSpecifier InfStartSound = new SoundPathSpecifier("/Audio/Ambience/Antag/headrev_start.ogg");
}
