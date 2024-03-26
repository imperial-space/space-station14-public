using System.Numerics;
using Content.Shared.Movement.Systems;
using Content.Shared.Access.Components;
using Content.Shared.Actions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Actions;

namespace Content.Shared.Imperial.SST;

public sealed class SSTSystem : EntitySystem
{

    [Dependency] private readonly SharedActionsSystem _action = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SSTComponent, MapInitEvent>(OnSSTMapInit);
    }

    private void OnSSTMapInit(Entity<SSTComponent> sst, ref MapInitEvent args)
    {
        foreach (var actionId in sst.Comp.ActionIds)
        {
            if (!sst.Comp.Actions.ContainsKey(actionId) &&
                _action.AddAction(sst, actionId) is { } newAction)
            {
                sst.Comp.Actions[actionId] = newAction;
            }
        }

        Dirty(sst);
    }

}