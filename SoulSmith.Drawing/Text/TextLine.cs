using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SoulSmith.Drawing.Text
{
    internal class TextLine
    {
        public string Text { get; set; }
        public int SpaceWidth { get; set; }
        public float WordWidthMultiplier { get; set; }
        public TextLine(string text, float wordWidthMultiplier, int spaceWidth)
        {
            Text = text;
            WordWidthMultiplier = wordWidthMultiplier;
            SpaceWidth = spaceWidth;
        }
        public void DrawLine(IReadOnlyPosition position, IFontResource font, Color color, SpriteBatch spriteBatch)
        {
            font.DrawText(position, color, Text, Vector2.Zero, spriteBatch);
        }
    }
}
