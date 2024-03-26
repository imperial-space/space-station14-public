using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Imperial.SST.BootSweep;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SSTBootSweepSystem))]
public sealed partial class SSTBootSweepComponent : Component
{
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public float Range = 1;

    [DataField]
    public DamageSpecifier? Damage;

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan StunTime = TimeSpan.FromSeconds(3);

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier Sound = new SoundCollectionSpecifier("BaseWhistle");

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId HitEffect = "SSTEffectPunch";

    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier HitSound = new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/bang.ogg");
}