using System.Numerics;
using Content.Shared.Movement.Systems;

namespace Content.Shared.Imperial.SST.Zoom;

public sealed class SSTZoomSystem : EntitySystem
{
    [Dependency] private readonly SharedContentEyeSystem _eye = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SSTZoomComponent, SSTZoomActionEvent>(OnSSTZoomAction);
    }

    private void OnSSTZoomAction(Entity<SSTZoomComponent> sst, ref SSTZoomActionEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        sst.Comp.Enabled = !sst.Comp.Enabled;
        Dirty(sst);

        if (sst.Comp.Enabled)
        {
            _eye.SetMaxZoom(sst, sst.Comp.Zoom);
            _eye.SetZoom(sst, sst.Comp.Zoom);
            return;
        }

        _eye.ResetZoom(sst);
    }
}