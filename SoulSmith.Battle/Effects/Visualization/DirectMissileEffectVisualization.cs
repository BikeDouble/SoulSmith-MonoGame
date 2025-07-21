using SoulSmith.Asset;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;
using System.Text.Json;
using SoulSmith.Vector;

namespace SoulSmith.Battle.Effects.Visualization
{
    public class DirectMissileEffectVisualization : EffectVisualization
    {
        // Children
        private CanvasObject _missile;
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private DrawableResourceKey _missileResourceKey;
        private Vector2? _missileSizeInPixels;

        public DirectMissileEffectVisualization(DrawableResourceKey missileResourceKey,
            float lifespan,
            float effectActivationTimer = -1,
            float baseDelay = 0f,
            Vector2? missileSizeInPixels = null) : base(lifespan, effectActivationTimer, baseDelay)
        {
            _missileSizeInPixels = missileSizeInPixels;
            _missileResourceKey = missileResourceKey;
            IAssetWrapper<IDrawableResource> missileAsset = DrawHelpers.GetDrawableResource(_missileResourceKey);

            _missile = new CanvasObject(null, missileAsset);
            AddChild(_missile);
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0)
        {
            base.BeginVisualization(sender, target, delay);

            if (Sender.FireZone != null)
            {
                _startPoint = Sender.FireZone.GetRandomGlobalPoint(Sender.GetGlobalPosition());
            }
            else
            {
                _startPoint = Sender.GetGlobalPosition().Coordinates;
            }

            _missile.SetCoordinates(_startPoint);

            if (_missileSizeInPixels.HasValue) _missile.ScaleToSetSize(_missileSizeInPixels.Value);

            if (Target.HitZone != null)
            {
                _endPoint = Target.HitZone.GetRandomGlobalPoint(Target.GetGlobalPosition());
            }
            else
            {
                _endPoint = Target.GetGlobalPosition().Coordinates;
            }
        }

        protected override void EnabledProcess(double delta)
        {
            base.EnabledProcess(delta);

            double interpolant = Math.Clamp(ElapsedLifespan / TotalLifespan, 0, 1);
            Vector2 difference = _endPoint - _startPoint;
            Vector2 desiredPosition = _startPoint + (float)interpolant * difference;

            _missile.SetCoordinates(desiredPosition);
        }
    }
}
