using SoulSmith.Asset;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;

namespace SoulSmith.Battle.Effect.Visualization
{
    public class GrowAndFadeOnTargetEffectVisualization : EffectVisualization
    {
        // Children
        private CanvasObject _growAndFader;
        private Vector2 _startScale;
        private Vector2 _endScale;

        public GrowAndFadeOnTargetEffectVisualization(IReadOnlyTrackedAsset<IDrawableResource> missileResource,
            float lifespan,
            float effectActivationTimer = -1,
            float delay = 0f) : base(lifespan, effectActivationTimer, delay)
        {
            _growAndFader = new CanvasObject(null, missileResource);
            AddChild(_growAndFader);
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0)
        {
            base.BeginVisualization(sender, target, delay);

            Vector2 startPoint = Target.HitZone.GetRandomGlobalPoint(Target.GetGlobalPosition());

            _growAndFader.Set(startPoint);

            _growAndFader.SetScale(Vector2.Zero);

            _startScale = Vector2.Zero;

            _endScale = 4 * _startScale;
        }

        protected override void EnabledProcess(double delta)
        {
            base.EnabledProcess(delta);

            float progress = (float)(delta / TotalLifespan);

            float alphaChange = -(255f * progress);

            _growAndFader.ChangeColorAdditive(0, 0, 0, (int)alphaChange);

            Vector2 curScale = (_endScale - _startScale) * progress + _startScale;

            _growAndFader.SetScale(curScale);
        }
    }
}