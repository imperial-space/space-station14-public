using System.Numerics;

namespace Content.Shared.Imperial.Modsuit.Zoom;

[ByRefEvent]
public record struct ModsuitGetZoomEvent(Vector2 Zoom)
{
    public void Increase(float zoom)
    {
        if (Zoom.X >= zoom)
            return;

        Zoom = new Vector2(zoom, zoom);
    }
}
