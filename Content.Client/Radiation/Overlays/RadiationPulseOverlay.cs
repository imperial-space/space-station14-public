using System.Linq;
using Content.Shared.Radiation.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Radiation.Overlays
{
    public sealed class RadiationPulseOverlay : Overlay
    {
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IGameTiming _gameTiming = default!;

        private const float MaxDist = 15.0f;

        public override OverlaySpace Space => OverlaySpace.WorldSpace;
        public override bool RequestScreenTexture => true;

        private readonly ShaderInstance _baseShader;
        private readonly Dictionary<EntityUid, (ShaderInstance shd, RadiationShaderInstance instance)> _pulses = new();

        public RadiationPulseOverlay()
        {
            IoCManager.InjectDependencies(this);
            _baseShader = _prototypeManager.Index<ShaderPrototype>("Radiation").Instance().Duplicate();
        }

        protected override bool BeforeDraw(in OverlayDrawArgs args)
        {
            RadiationQuery(args.Viewport.Eye);
            return _pulses.Count > 0;
        }

        protected override void Draw(in OverlayDrawArgs args)
        {
            if (ScreenTexture == null)
                return;

            var worldHandle = args.WorldHandle;
            var viewport = args.Viewport;

            foreach ((var shd, var instance) in _pulses.Values)
            {
                if (instance.CurrentMapCoords.MapId != args.MapId)
                    continue;

                // To be clear, this needs to use "inside-viewport" pixels.
                // In other words, specifically NOT IViewportControl.WorldToScreen (which uses outer coordinates).
                var tempCoords = viewport.WorldToLocal(instance.CurrentMapCoords.Position);
                tempCoords.Y = viewport.Size.Y - tempCoords.Y;
                shd?.SetParameter("renderScale", viewport.RenderScale);
                shd?.SetParameter("positionInput", tempCoords);
                shd?.SetParameter("range", instance.Range);
                var life = (_gameTiming.RealTime - instance.Start).TotalSeconds / instance.Duration;
                shd?.SetParameter("life", (float) life);

                // There's probably a very good reason not to do this.
                // Oh well!
                shd?.SetParameter("SCREEN_TEXTURE", viewport.RenderTarget.Texture);

                worldHandle.UseShader(shd);
                worldHandle.DrawRect(Box2.CenteredAround(instance.CurrentMapCoords.Position, new Vector2(instance.Range, instance.Range) * 2f), Color.White);
            }

            worldHandle.UseShader(null);
        }

        //Queries all pulses on the map and either adds or removes them from the list of rendered pulses based on whether they should be drawn (in range? on the same z-level/map? pulse entity still exists?)
        private void RadiationQuery(IEye? currentEye)
        {
            if (currentEye == null)
            {
                _pulses.Clear();
                return;
            }

            var currentEyeLoc = currentEye.Position;

            var pulses = _entityManager.EntityQuery<RadiationPulseComponent>();
            foreach (var pulse in pulses) //Add all pulses that are not added yet but qualify
            {
                var pulseEntity = pulse.Owner;

                if (!_pulses.ContainsKey(pulseEntity) && PulseQualifies(pulseEntity, currentEyeLoc))
                {
                    _pulses.Add(
                            pulseEntity,
                            (
                                _baseShader.Duplicate(),
                                new RadiationShaderInstance(
                                    _entityManager.GetComponent<TransformComponent>(pulseEntity).MapPosition,
                                    pulse.VisualRange,
                                    pulse.StartTime,
                                    pulse.VisualDuration
                                )
                            )
                    );
                }
            }

            var activeShaderIds = _pulses.Keys;
            foreach (var pulseEntity in activeShaderIds) //Remove all pulses that are added and no longer qualify
            {
                if (_entityManager.EntityExists(pulseEntity) &&
                    PulseQualifies(pulseEntity, currentEyeLoc) &&
                    _entityManager.TryGetComponent<RadiationPulseComponent?>(pulseEntity, out var pulse))
                {
                    var shaderInstance = _pulses[pulseEntity];
                    shaderInstance.instance.CurrentMapCoords = _entityManager.GetComponent<TransformComponent>(pulseEntity).MapPosition;
                    shaderInstance.instance.Range = pulse.VisualRange;
                } else {
                    _pulses[pulseEntity].shd.Dispose();
                    _pulses.Remove(pulseEntity);
                }
            }

        }

        private bool PulseQualifies(EntityUid pulseEntity, MapCoordinates currentEyeLoc)
        {
            return _entityManager.GetComponent<TransformComponent>(pulseEntity).MapID == currentEyeLoc.MapId && _entityManager.GetComponent<TransformComponent>(pulseEntity).Coordinates.InRange(_entityManager, EntityCoordinates.FromMap(_entityManager, _entityManager.GetComponent<TransformComponent>(pulseEntity).ParentUid, currentEyeLoc), MaxDist);
        }

        private sealed record RadiationShaderInstance(MapCoordinates CurrentMapCoords, float Range, TimeSpan Start, float Duration)
        {
            public MapCoordinates CurrentMapCoords = CurrentMapCoords;
            public float Range = Range;
            public TimeSpan Start = Start;
            public float Duration = Duration;
        };
    }
}

