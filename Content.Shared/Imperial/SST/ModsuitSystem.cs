using System.Numerics;
using Content.Shared.Movement.Systems;
using Content.Shared.Access.Components;
using Content.Shared.Actions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Actions;

namespace Content.Shared.Imperial.Modsuit;

public sealed class ModsuitSystem : EntitySystem
{

    [Dependency] private readonly SharedActionsSystem _action = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ModsuitComponent, MapInitEvent>(OnModsuitMapInit);
    }

    private void OnModsuitMapInit(Entity<ModsuitComponent> modsuit, ref MapInitEvent args)
    {
        foreach (var actionId in modsuit.Comp.ActionIds)
        {
            if (!modsuit.Comp.Actions.ContainsKey(actionId) &&
                _action.AddAction(modsuit, actionId) is { } newAction)
            {
                modsuit.Comp.Actions[actionId] = newAction;
            }
        }

        Dirty(modsuit);
    }

}