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

        public void UpdateState(string newState, bool force = false) { _text = newState; }

        public void Dispose()
        {
            _wrappedFont?.Dispose();
        }

        private Vector2 GetOriginInternal()
        {
            switch (OriginPlacement)
            {
                case OriginPlacement.TopLeft:
                    return Vector2.Zero;
                case OriginPlacement.Center:
                    return _wrappedFont.Value.MeasureString(_text) / 2;
                case OriginPlacement.BottomMiddle:
                    Vector2 size = _wrappedFont.Value.MeasureString(_text);
                    return new Vector2(size.X / 2, size.Y);
                case OriginPlacement.TopMiddle:
                    size = _wrappedFont.Value.MeasureString(_text);
                    return new Vector2(size.X / 2, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(OriginPlacement), OriginPlacement, "Invalid origin placement specified.");
            }
        }

        public void Process(double delta) { }

        public Vector2 Origin { get { return GetOriginInternal(); } }
        public OriginPlacement OriginPlacement { get; set; } = OriginPlacement.Center;
        public int Width { get { return (int)_wrappedFont.Value.MeasureString(_text).X; } }
        public int Height { get { return (int)_wrappedFont.Value.MeasureString(_text).Y; } }
    }
}
