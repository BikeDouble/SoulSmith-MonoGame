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
    public class ZonedResource : IZone, IDrawableResource
    {
        private IZone _clickZone;
        private IDrawableResource _resource;

        public ZonedResource(IZone clickZone, IDrawableResource resource)
        {
            _clickZone = clickZone;
            _resource = resource;
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation = null)
        {
            if (_clickZone == null) return false;

            return _clickZone.ContainsGlobal(point, transformation);
        }

        public void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch)
        {
            _resource.Draw(position, tint, spriteBatch);
        }

        public void Dispose() 
        {
            _resource.Dispose();
        }
    }
}
