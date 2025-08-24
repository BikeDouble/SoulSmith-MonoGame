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
        private string _missileResourceKey;
        private Vector2? _missileSizeInPixels;

        public DirectMissileEffectVisualization(string missileResourceKey,
            float lifespan,
            float effectActivationTimer = -1,
            float baseDelay = 0f,
            Vector2? missileSizeInPixels = null) : base(lifespan, effectActivationTimer, baseDelay)
        {
            _missileSizeInPixels = missileSizeInPixels;
            _missileResourceKey = missileResourceKey;
            IDrawableResource missileAsset = DrawHelpers.GetDrawableResourceInstance(_missileResourceKey);

            _missile = new CanvasObject(null, missileAsset);
            AddChild(_missile);
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0, bool mirrorSprites = false)
        {
            base.BeginVisualization(sender, target, delay, mirrorSprites);

            IReadOnlyCanvasObject senderSprite = sender.ReadOnlySprite;

            _startPoint = senderSprite.GetRandomGlobalPoint(Sender.GetGlobalPosition(), FIREZONEZONEKEY);

            _missile.SetCoordinates(_startPoint);

            if (_missileSizeInPixels.HasValue) _missile.ScaleToSetSize(_missileSizeInPixels.Value);

            _endPoint = senderSprite.GetRandomGlobalPoint(Target.GetGlobalPosition(), HITZONEZONEKEY);

            if (MirrorSprites)
            {
                _missile.Scale(new Vector2(-1, 1));
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
