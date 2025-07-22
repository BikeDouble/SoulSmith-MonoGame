using Microsoft.Xna.Framework;

namespace SoulSmith.Core
{
    public class Position : IReadOnlyPosition
    {
        public Position(int x = 0, int y = 0, float width = 1f, float height = 1f, float rotation = 0f, int z = 0)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Rotation = rotation;
            Z = z;
            if (Width == 0f) Width = MINIMUMSCALE;
            if (Height == 0f) Height = MINIMUMSCALE;
        }

        public Position(Vector2 coordinates)
        {
            Coordinates = coordinates;
        }

        public Position(IReadOnlyPosition other)
        {
            if (other == null)
                return;

            ScaleVector = other.ScaleVector;
            Coordinates = other.Coordinates;
            Rotation = other.Rotation;
            Z = other.Z;
            if (Width == 0f) Width = MINIMUMSCALE;
            if (Height == 0f) Height = MINIMUMSCALE;
        }

        public void Transform(IReadOnlyPosition transformation)
        {
            Scale(transformation.ScaleVector);
            Translate(transformation.Coordinates);
            Rotate(transformation.Rotation);
            ZTranslate(transformation.Z);
        }

        public void TransformInContext(IReadOnlyPosition transformation, IReadOnlyPosition context)
        {
            Scale(transformation.ScaleVector);
            Translate(transformation.Coordinates * context.ScaleVector); //TODO account for context rotation?
            Rotate(transformation.Rotation);
            ZTranslate(transformation.Z);
        }

        public void Translate(Vector2 translation)
        {
            Coordinates += translation;
        }

        public void ZTranslate(int zTranslation)
        {
            Z += zTranslation;
        }

        public void Scale(Vector2 scale)
        {
            ScaleVector *= scale;
            if (Width == 0f) Width = MINIMUMSCALE;
            if (Height == 0f) Height = MINIMUMSCALE;
        }

        public void SetScale(Vector2 scale)
        {
            ScaleVector = scale;
            if (Width == 0f) Width = MINIMUMSCALE;
            if (Height == 0f) Height = MINIMUMSCALE;
        }

        public void Rotate(float rotation, Vector2? origin = null)
        {
            Vector2 originVal = origin ?? Vector2.Zero;

            if (originVal != Vector2.Zero) Coordinates = RotatePointAroundPoint(Coordinates, originVal, rotation);

            Rotation += rotation;
        }

        public static Vector2 RotatePointAroundPoint(Vector2 point, Vector2 origin, float rotation)
        {
            Vector2 relativePos = point - origin;

            float newX = (float)((relativePos.X * Math.Cos(rotation)) - (relativePos.Y * Math.Sin(rotation)));
            float newY = (float)((relativePos.Y * Math.Cos(rotation)) + (relativePos.X * Math.Sin(rotation)));

            Vector2 newRelativePos = new Vector2(newX, newY);

            return newRelativePos + origin;
        }

        public void SetCoordinates(Vector2 coordinates)
        {
            Coordinates = coordinates;
        }

        public void SetZ(int z)
        {
            Z = z;
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public const float MAXROTATION = (float)(Math.PI * 2);
        public const float MINIMUMSCALE = 0.0001f;

        private Vector2 _scale = Vector2.One;
        private Vector2 _coordinates = Vector2.Zero;
        private int _z = 0;
        private float _rotation = 0f;

        public Vector2 ScaleVector { get { return _scale; } private set { _scale = value; } }
        public float Width { get { return _scale.X; } private set { _scale.X = value; } }
        public float Height { get { return _scale.Y; } private set { _scale.Y = value; } }
        public Vector2 Coordinates { get { return _coordinates; } private set { _coordinates = value; } }
        public float Rotation
        {
            get { return _rotation; }
            private set
            {
                _rotation = value;

                if (_rotation >= MAXROTATION)
                {
                    _rotation -= MAXROTATION;
                }

                if (_rotation < 0)
                {
                    _rotation += MAXROTATION;
                }
            }
        }

        public int X { get { return (int)Coordinates.X; } private set { _coordinates.X = value; } }
        public int Y { get { return (int)Coordinates.Y; } private set { _coordinates.Y = value; } }
        public int Z { get { return _z; } private set { _z = value; } }
        public static Position operator +(Position a, Position b)
           => new Position(a.X + b.X, a.Y + b.Y, a.Width * b.Width, a.Height * b.Height, a.Rotation + b.Rotation, a.Z + b.Z);
    }


}
