using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private string _nextClipName = null;
        private double _animationSpeed = 1d;

        public SamplerState SamplerState { get { return _wrappedAnimation.Value.SamplerState; } }

        public AnimationInstance(IAssetWrapper<Animation> wrappedAnimation, double animationSpeed = 1d)
        {
            _wrappedAnimation = wrappedAnimation;
            _currentClipName = _wrappedAnimation.Value.GetDefaultClipName();
        }

        public void Draw(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch)
        {
            _wrappedAnimation.Value.DrawFrame(position, color, spriteBatch, _currentClipName, _timeSinceClipChange, _animationSpeed, OriginPlacement);
        }

        public void UpdateState(string newState, bool force = false) //TODO implement safe frames
        {
            if (_wrappedAnimation.Value.ContainsClip(newState) && (newState != _currentClipName))
            {
                if (force || _wrappedAnimation.Value.IsOnTransitionFrame(_currentClipName, _timeSinceClipChange, _animationSpeed))
                {
                    _currentClipName = newState;
                    _timeSinceClipChange = 0;
                }
                else
                {
                    _nextClipName = newState;
                }
            }
        }

        public void ChangeSpeed(double newSpeed)
        {
            _animationSpeed = newSpeed;
        }

        public void Process(double delta)
        {
            _timeSinceClipChange += delta;

            if (_nextClipName != null)
            {
                if (_wrappedAnimation.Value.IsOnTransitionFrame(_currentClipName, _timeSinceClipChange, _animationSpeed))
                {
                    _currentClipName = _nextClipName;
                    _nextClipName = null;
                    _timeSinceClipChange = 0;
                }
            }
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
