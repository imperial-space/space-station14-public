using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared.Imperial.SST.Zoom;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SSTZoomSystem))]
public sealed partial class SSTZoomComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Enabled;

    [DataField, AutoNetworkedField]
    public Vector2 Zoom = new(1.25f, 1.25f);
}
