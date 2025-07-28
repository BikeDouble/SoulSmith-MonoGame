using SoulSmith.Asset;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;
using SoulSmith.Core;

namespace SoulSmith.Battle.Effects.Visualization
{
    public class GrowAndFadeOnTargetEffectVisualization : EffectVisualization
    {
        // Children
        private CanvasObject _particle;
        private Vector2 _startSize;
        private Vector2 _endSize;

        public GrowAndFadeOnTargetEffectVisualization(string particleResourceKey,
            Vector2 startSize,
            Vector2 endSize,
            float lifespan,
            float effectActivationTimer = -1,
            float delay = 0f) : base(lifespan, effectActivationTimer, delay)
        {
            IDrawableResource particleResource = DrawHelpers.GetDrawableResourceInstance(particleResourceKey);
            _particle = new CanvasObject(null, particleResource);
            AddChild(_particle);
            _startSize = startSize;
            _endSize = endSize;
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0)
        {
            base.BeginVisualization(sender, target, delay);

            Vector2 startPoint;

            if (Target.HitZone != null)
            {
                startPoint = Target.HitZone.GetRandomGlobalPoint(Target.GetGlobalPosition());
            }
            else
            {
                startPoint = Target.GetGlobalPosition().Coordinates;
            }

            int rotation = Rand.RandInt(360);

            _particle.Rotate(rotation);

            _particle.ScaleToSetSize(_startSize);

            _particle.SetCoordinates(startPoint);
        }

        protected override void EnabledProcess(double delta)
        {
            base.EnabledProcess(delta);

            float progress = (float)(ElapsedLifespan / TotalLifespan);

            int currentAlpha = 255 - (int)(255 * progress);

            _particle.SetColor(255, 255, 255, currentAlpha);

            Vector2 curSize = (_endSize - _startSize) * progress + _startSize;

            _particle.ScaleToSetSize(curSize);
        }
    }
}