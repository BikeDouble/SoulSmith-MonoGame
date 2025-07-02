using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Shapes;

namespace SoulSmith.Drawing
{
    public class ZonedResource : IZone, IDrawableResource, IAsset
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

            Position newTransformation = new Position(transformation);

            newTransformation.Translate(Origin * transformation.ScaleVector * -1);

            return _clickZone.ContainsGlobal(point, newTransformation);
        }

        public bool ContainsLocal(Vector2 point)
        {
            if (_clickZone == null) { return false; }

            Position originTransformation = new Position(Origin * -1);

            return _clickZone.ContainsGlobal(point, originTransformation);
        }

        public Vector2 GetRandomLocalPoint()
        {
            return _clickZone.GetRandomLocalPoint();
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position)
        {
            return _clickZone.GetRandomGlobalPoint(position);
        }

        public float GetAreaLocal()
        {
            return _clickZone.GetAreaLocal();
        }

        public void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch)
        {
            _resource.Draw(position, tint, spriteBatch);
        }

        public void Dispose() 
        {
            _resource.Dispose();
        }

        public int Width { get { return _resource.Width; } }
        public int Height { get { return _resource.Height; } }
        public Vector2 Origin { get { return _resource.Origin; } }
    }
}
