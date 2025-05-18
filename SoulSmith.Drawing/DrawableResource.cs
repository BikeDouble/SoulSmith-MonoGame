

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public class DrawableResource : IDeepCloneable, IDrawableResource
    {
        private Color _color = Color.Black;

        public DrawableResource() { }

        public DrawableResource(DrawableResource other)
        {
            _color = other._color;
        }

        public DrawableResource(Color color)
        {
            _color = color;
        }

        public virtual void Draw(IReadOnlyCanvasPosition position, Vector4 tint, SpriteBatch spriteBatch) { }

        public virtual void UpdateText(string text) { }

        public void AddColor(int r, int g, int b, int a)
        {
            UpdateColor(new Color(
                _color.R + r,
                _color.G + g,
                _color.B + b,
                _color.A + a));
        }

        public void UpdateColor(Color color)
        {
            _color = color;
        }

        public void SetAlpha(int a)
        {
            _color.A = (byte)Math.Clamp(a, 0, 255);
        }

        public virtual object DeepClone()
        {
            return new DrawableResource(this);
        }

        public virtual bool ContainsPoint(Vector2 point, IReadOnlyCanvasPosition position)
        {
            return false;
        }

        public Color GetTintedColor(Vector4 tint)
        {
            return new Color(
                _color.R + (int)tint.X,
                _color.G + (int)tint.Y,
                _color.B + (int)tint.Z,
                _color.A + (int)tint.W);
        }

        public Color Color { get { return _color; } }
    }
}