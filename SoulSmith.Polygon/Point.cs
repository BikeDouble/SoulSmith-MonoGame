using SoulSmith.Core;
using Microsoft.Xna.Framework;

namespace SoulSmith.Shapes
{
    public class Point : IZone
    {
        private Vector2 _position;

        public Point(Vector2 position)
        {
            _position = position;
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation) 
        {
            return ContainsLocal(point - transformation.Coordinates);
        }

        public bool ContainsLocal(Vector2 point)
        {
            if ((Math.Abs(_position.X - point.X) <= 0.5) && (Math.Abs(_position.Y - point.Y) <= 0.5))
            {
                return true;
            }

            return false;
        }

        public Vector2 GetRandomLocalPoint()
        {
            return _position;
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition transformation)
        {
            return _position + transformation.Coordinates;
        }

        public float GetAreaLocal()
        {
            return 1;
        }
    }
}
