namespace Content.Server.Destructible.Thresholds.Behaviors;


[DataDefinition]
public sealed partial class FireEntity : IThresholdBehavior
{

    public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
    {
        Console.WriteLine(owner);

        if (system.EntityManager.TryGetComponent(owner, out TransformComponent? tr))
        {
            system.EntityManager.SpawnEntity("AnimatedAsh", tr.Coordinates);
            system.EntityManager.QueueDeleteEntity(owner);
        }
    }
}
