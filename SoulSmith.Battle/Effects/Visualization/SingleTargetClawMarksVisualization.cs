using Microsoft.Xna.Framework;
using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Object;
using SoulSmith.Object.Canvas;
using System;

namespace SoulSmith.Battle.Effects.Visualization
{
    public class SingleTargetClawMarksVisualization : EffectVisualization
    {
        private Vector2 _size;
        private Color _color;
        private float _slashTime;
        private float _lingerTime;
        private float _fadeTime;

        // Children
        private ScissorRect _particleScissorRect;

        public SingleTargetClawMarksVisualization(string particleResourceKey,
            Vector2 size,
            Color color,
            float slashTime,
            float lingerTime,
            float fadeTime,
            float lifespan,
            float effectActivationTimer,
            float delay = 0f) : base(lifespan, effectActivationTimer, delay)
        {
            _size = size;
            _color = color;
            _slashTime = slashTime;
            _lingerTime = lingerTime;
            _fadeTime = fadeTime;

            IDrawableResource particleResource = DrawHelpers.GetDrawableResourceInstance(particleResourceKey);

            CanvasObject particle = new CanvasObject(new Position(_size / 2), particleResource);
            particle.ScaleToSetSize(size);

            _particleScissorRect = new ScissorRect(null, (int)_size.X, (int)0, null, new List<SoulSmithObject>{ particle });
            _particleScissorRect.SetOriginPlacement(OriginPlacement.TopLeft);
            this.SetColor(color);

            AddChild(_particleScissorRect);
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0, bool mirrorSprites = false)
        {
            base.BeginVisualization(sender, target, delay, mirrorSprites);

            Vector2 startPoint;

            IReadOnlyUnitSprite targetSprite = target.ReadOnlySprite;

            startPoint = Target.GetGlobalPosition().Coordinates;

            if (MirrorSprites)
            {
                _particleScissorRect.Scale(new Vector2(-1, 1));
                _particleScissorRect.SetCoordinates(startPoint - (new Vector2(-_size.X / 2, _size.Y / 2)));
            }
            else
            {
                _particleScissorRect.SetCoordinates(startPoint - (_size / 2));
            }
        }

        protected override void EnabledProcess(double delta)
        {
            base.EnabledProcess(delta);

            float slashProgress = Math.Clamp((ElapsedLifespan / _slashTime), 0f, 1f);
            float fadeProgress = Math.Clamp(((ElapsedLifespan - _slashTime - _lingerTime) / _fadeTime), 0f, 1f);

            // Slash
            int currentY = (int)(_size.Y * slashProgress);

            // Fade
            int currentAlpha = 255 - (int)(255 * fadeProgress);

            _particleScissorRect.SetColor(255, 255, 255, currentAlpha);

            _particleScissorRect.Height = currentY;
        }
    }
}