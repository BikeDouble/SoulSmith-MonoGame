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
    public class SimpleTextInstance : IDrawableTextResource
    {
        private IAssetWrapper<IFontResource> _wrappedFont;
        private string _text = string.Empty;

        public SimpleTextInstance(IAssetWrapper<IFontResource> font)
        {
            _wrappedFont = font;
        }

        public void Draw(IReadOnlyPosition postion, Color color, SpriteBatch spriteBatch) 
        {
            if (_wrappedFont == null) throw new ArgumentNullException(nameof(_wrappedFont));

            if (_wrappedFont.Value == null) throw new ArgumentNullException(nameof(_wrappedFont.Value));

            _wrappedFont.Value.DrawText(postion, color, _text, Origin, spriteBatch);
        }

        public void UpdateState(string newState) { _text = newState; }

        public void Dispose()
        {
            _wrappedFont?.Dispose();
        }

        public void Process(double delta) { }

        public Vector2 Origin { get { return _wrappedFont.Value.MeasureString(_text)/2; } }
        public int Width { get { return (int)_wrappedFont.Value.MeasureString(_text).X; } }
        public int Height { get { return (int)_wrappedFont.Value.MeasureString(_text).Y; } }
    }
}
