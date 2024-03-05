using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Interaction;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Content.Shared.Traits.Assorted;
using Robust.Shared.Audio.Systems;

namespace Content.Shared.Imperial.SST.BootSweep;

public sealed class SSTBootSweepSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly RotateToFaceSystem _rotateTo = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private readonly HashSet<Entity<PsychosisGainComponent>> _hit = new(); //funny

    public override void Initialize()
    {
        SubscribeLocalEvent<SSTBootSweepComponent, SSTBootSweepActionEvent>(OnSSTBootSweepAction);
    }

    private void OnSSTBootSweepAction(Entity<SSTBootSweepComponent> sst, ref SSTBootSweepActionEvent args)
    {
        if (!TryComp(sst, out TransformComponent? transform))
            return;

        var ev = new BootSweepAttemptEvent();
        RaiseLocalEvent(sst, ref ev);

        if (ev.Cancelled)
            return;

        args.Handled = true;

        _audio.PlayPredicted(sst.Comp.Sound, sst, sst);
        EnsureComp<SSTBootSweepingComponent>(sst);

        if (_net.IsClient)
            return;

        _hit.Clear();
        _entityLookup.GetEntitiesInRange(transform.Coordinates, 1.5f, _hit);

        var origin = _transform.GetMapCoordinates(sst);
        foreach (var humanoid in _hit)
        {
        if (!HasComp<SSTComponent>(humanoid))
        {
            var humanoidCoords = _transform.GetMapCoordinates(humanoid);
            var diff = humanoidCoords.Position - origin.Position;
            diff *= sst.Comp.Range / 3;

            if (sst.Comp.Damage is { } damage)
                _damageable.TryChangeDamage(humanoid, damage);

            var filter = Filter.Pvs(humanoid, entityManager: EntityManager);

            _throwing.TryThrow(humanoid, diff, 5);
            _stun.TryKnockdown(humanoid, sst.Comp.StunTime, true);
            _stun.TryStun(humanoid, sst.Comp.StunTime, true);

            _audio.PlayPvs(sst.Comp.HitSound, humanoid);
            SpawnAttachedTo(sst.Comp.HitEffect, humanoid.Owner.ToCoordinates());
        }
    }
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<SSTBootSweepingComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var sweeping, out var xform))
        {
            if (sweeping.NextRotation > _timing.CurTime)
                continue;

            if (sweeping.TotalRotations >= sweeping.MaxRotations)
            {
                RemCompDeferred<SSTBootSweepingComponent>(uid);
                continue;
            }

            sweeping.TotalRotations++;
            sweeping.NextRotation = _timing.CurTime + sweeping.Delay;
            sweeping.LastDirection ??= _transform.GetWorldRotation(xform).GetDir();

            var nextAngle = sweeping.LastDirection.Value.ToAngle() + Angle.FromDegrees(90);
            sweeping.LastDirection = nextAngle.GetDir();

            Dirty(uid, sweeping);

            _rotateTo.TryFaceAngle(uid, nextAngle, xform);
        }
    }

    public override void FrameUpdate(float frameTime)
    {
        var query = EntityQueryEnumerator<SSTBootSweepingComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var sweeping, out var xform))
        {
            if (sweeping.LastDirection is not { } direction)
                continue;

            _rotateTo.TryFaceAngle(uid, direction.ToAngle(), xform);
        }
    }
}
