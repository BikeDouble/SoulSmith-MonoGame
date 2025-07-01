using SoulSmith.Asset;
using SoulSmith.Battle;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;

namespace SoulSmith.Effect.Visualization
{
    public class DirectMissileEffectVisualization : EffectVisualization
    {
        // Children
        private CanvasObject _missile;
        private Vector2 _startPoint;
        private Vector2 _endPoint;

        public DirectMissileEffectVisualization(IReadOnlyTrackedAsset<IDrawableResource> missileResource, 
            float lifespan, 
            float effectActivationTimer = -1,
            float delay = 0f) : base(lifespan, effectActivationTimer, delay)
        {
            _missile = new CanvasObject(null, missileResource);
            AddChild(_missile);
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0)
        {
            base.BeginVisualization(sender, target, delay);

            _startPoint = Sender.HitZone.GetRandomGlobalPoint(Sender.GetGlobalPosition());

            _missile.Set(_startPoint);

            _endPoint = Target.HitZone.GetRandomGlobalPoint(Target.GetGlobalPosition());
        }

        protected override void EnabledProcess(double delta)
        {
            base.EnabledProcess(delta);

            double interpolant = Math.Clamp(ElapsedLifespan / TotalLifespan, 0, 1);
            Vector2 difference = _endPoint - _startPoint;
            Vector2 desiredPosition = _startPoint + ((float)interpolant * difference);

            _missile.Set(desiredPosition);
        }
    }
}
