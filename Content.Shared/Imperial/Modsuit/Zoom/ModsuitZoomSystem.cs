using System.Numerics;
using Content.Shared.Movement.Systems;

namespace Content.Shared.Imperial.Modsuit.Zoom;

public sealed class ModsuitZoomSystem : EntitySystem
{
    [Dependency] private readonly SharedContentEyeSystem _eye = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ModsuitZoomComponent, ModsuitZoomActionEvent>(OnModsuitZoomAction);
    }

    private void OnModsuitZoomAction(EntityUid<ModsuitZoomComponent> modsuit, ref ModsuitZoomActionEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        modsuit.Comp.Enabled = !modsuit.Comp.Enabled;
        Dirty(modsuit);

        if (modsuit.Comp.Enabled)
        {
            _eye.SetMaxZoom(modsuit, modsuit.Comp.Zoom);
            _eye.SetZoom(modsuit, modsuit.Comp.Zoom);
            return;
        }

        _eye.ResetZoom(modsuit);
    }
}
