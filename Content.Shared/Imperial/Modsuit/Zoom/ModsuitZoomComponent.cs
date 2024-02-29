using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared.Imperial.Modsuit.Zoom;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(ModsuitZoomSystem))]
public sealed partial class ModsuitZoomComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Enabled;

    [DataField, AutoNetworkedField]
    public Vector2 Zoom = new(1.25f, 1.25f);
}
