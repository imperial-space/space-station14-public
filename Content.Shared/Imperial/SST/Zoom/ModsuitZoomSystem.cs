using System.Numerics;
using Content.Shared.Movement.Systems;
using Content.Shared.Clothing;
using Content.Shared.Actions;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Slippery;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Robust.Shared.Containers;

namespace Content.Shared.Imperial.Modsuit.Zoom;

public sealed class ModsuitZoomSystem : EntitySystem
{
    [Dependency] private readonly SharedContentEyeSystem _eye = default!;
    [Dependency] private readonly SharedActionsSystem _sharedActions = default!;
    [Dependency] private readonly SharedActionsSystem _actionContainer = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ModsuitZoomComponent, ModsuitZoomActionEvent>(OnModsuitZoomAction);
        SubscribeLocalEvent<ModsuitZoomComponent, GetItemActionsEvent>(OnGetActions);
    }

    private void OnModsuitZoomAction(Entity<ModsuitZoomComponent> modsuit, ref ModsuitZoomActionEvent args)
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

   void OnGetActions(EntityUid uid, ModsuitZoomComponent component, GetItemActionsEvent args)
    {
        args.AddAction(ref component.ToggleActionEntity, component.ToggleAction);
    }

    }
}
