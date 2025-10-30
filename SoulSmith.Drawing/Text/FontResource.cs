using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;

namespace SoulSmith.Drawing.Text
{
    public class FontResource : IFontResource
    {
        public const int STANDARDFONTSIZE = 48;

        private FontSystem _fontSystem;

        public FontResource(FontSystem fontSystem) 
        {
            _fontSystem = fontSystem;
        }

        public void DrawText(IReadOnlyPosition position, Color color, string text, Vector2 origin, SpriteBatch spriteBatch)
        {
            origin *= position.Height;

            int fontSize = (int)(STANDARDFONTSIZE * position.Height);

            DynamicSpriteFont font = _fontSystem.GetFont(fontSize);

            font.DrawText(spriteBatch, text, position.Coordinates, color, position.Rotation, origin);
        }

        public Vector2 MeasureString(string text, int? fontSize = null)
        {
            fontSize ??= STANDARDFONTSIZE;

            DynamicSpriteFont font = _fontSystem.GetFont(fontSize.Value);

            return font.MeasureString(text);
        }

        public void Dispose()
        {
            _fontSystem?.Dispose();
        }
    }
}
