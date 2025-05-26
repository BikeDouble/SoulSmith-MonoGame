

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public class DrawableResource_Text : DrawableResource
    {
        private string _text = null;
        private SpriteFont _font;

        public DrawableResource_Text() : base() { }

        public DrawableResource_Text(SpriteFont font, string text = null)
        {
            _font = font;
            _text = text;
        }

        public DrawableResource_Text(DrawableResource_Text other) : base(other)
        {
            _font = other._font;
            _text = other._text;
        }

        public override void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch)
        {
            if (_font == null)
                return;

            if (_text == null)
                return;

            if (_text.Length == 0)
                return;

            DrawInternal(_text, _font, position, spriteBatch, GetTintedColor(tint));
        }

        public static void DrawInternal(string text, SpriteFont font, IReadOnlyPosition position, SpriteBatch spriteBatch, Color color, bool centered = true)
        {
            Vector2 coords = position.Coordinates;

            if (centered)
            {
                coords -= font.MeasureString(text) / 2;
            }

            spriteBatch.DrawString(font, text, coords, color, position.Rotation, Vector2.Zero, position.ScaleVector, SpriteEffects.None, 0);
        }

        public override object DeepClone()
        {
            return new DrawableResource_Text(this);
        }

        public override void UpdateText(string text)
        {
            _text = text;
        }
    }
}