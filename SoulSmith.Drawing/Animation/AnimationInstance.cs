using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Animation
{
    public class AnimationInstance : IDrawableResource
    {
        private IAssetWrapper<Animation> _wrappedAnimation;
        private double _timeSinceClipChange = 0;
        private string _currentClipName;
        private double _animationSpeed = 1d;

        public AnimationInstance(IAssetWrapper<Animation> wrappedAnimation, double animationSpeed = 1d)
        {
            _wrappedAnimation = wrappedAnimation;
            _currentClipName = _wrappedAnimation.Value.GetDefaultClipName();
        }

        public void Draw(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch)
        {
            _wrappedAnimation.Value.DrawFrame(position, color, spriteBatch, _currentClipName, _timeSinceClipChange, _animationSpeed, OriginPlacement);
        }

        public void UpdateState(string newState)
        {
            if (_wrappedAnimation.Value.ContainsClip(newState) && (newState != _currentClipName))
            {
                _currentClipName = newState;
                _timeSinceClipChange = 0;
            }
        }

        public void ChangeSpeed(double newSpeed)
        {
            _animationSpeed = newSpeed;
        }

        public void Process(double delta)
        {
            _timeSinceClipChange += delta;
        }

        public void Dispose()
        {
            _wrappedAnimation?.Dispose();
        }

        public double Speed { get { return _animationSpeed; } }
        public int Height { get { return _wrappedAnimation.Value.Height; } }
        public int Width { get { return _wrappedAnimation.Value.Width; } }
        public Vector2 Origin { get { return _wrappedAnimation.Value.GetFrameOrigin(OriginPlacement); } }
        public OriginPlacement OriginPlacement { get; set; } = OriginPlacement.Center;
    }
}
