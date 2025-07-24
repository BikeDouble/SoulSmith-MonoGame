using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Text
{
    public interface IFontResource : IDisposable
    {
        public void DrawText(IReadOnlyPosition position, Color color, string text, Vector2 origin, SpriteBatch spriteBatch);
        public Vector2 MeasureString(string text);
    }
}
