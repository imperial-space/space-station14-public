using Robust.Shared.GameStates;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Interaction;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;


namespace Content.Shared.Imperial.SST.BootSweep;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SSTBootSweepSystem))]
public sealed partial class SSTBootSweepingComponent : Component
{
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public Direction? LastDirection;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan Delay = TimeSpan.FromSeconds(0.07);

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextRotation;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public int TotalRotations;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public int MaxRotations = 4;
}