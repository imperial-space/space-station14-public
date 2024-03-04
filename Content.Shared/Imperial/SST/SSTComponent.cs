using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Imperial.SST;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SSTSystem))]
public sealed partial class SSTComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<EntProtoId> ActionIds = new();

    [DataField, AutoNetworkedField]
    public bool Enabled { get; set; } = true;

    [DataField]
    public Dictionary<EntProtoId, EntityUid> Actions = new();
}
