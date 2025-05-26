using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Core;
using SoulSmith.Shapes;

namespace SoulSmith.Drawing
{
    public class ClickableResource : IZone, IDrawableResource
    {
        private IZone _clickZone;
        private IDrawableResource _resource;

        public ClickableResource(IZone clickZone, IDrawableResource resource)
        {
            _clickZone = clickZone;
            _resource = resource;
        }

        public bool Contains(Vector2 point)
        {
            return _clickZone.Contains(point);
        }

        public void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch)
        {
            _resource.Draw(position, tint, spriteBatch);
        }
    }
}
