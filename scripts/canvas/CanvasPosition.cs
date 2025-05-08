using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

public class CanvasPosition : IReadOnlyCanvasPosition
{
    public CanvasPosition()
    {

    }

    public CanvasPosition(int x = 0, int y = 0, float width = 1f, float height = 1f, float rotation = 0f, int z = 0)
    {
        Width = width;
        Height = height;
        X = x;
        Y = y;
        Rotation = rotation;
        Z = z;
    }

    public CanvasPosition(Vector2 coordinates)
    {
        Coordinates = coordinates;
    }

    public CanvasPosition(CanvasPosition other)
    {
        if (other == null)
            return;

        ScaleVector = other.ScaleVector;
        Coordinates = other.Coordinates;
        Rotation = other.Rotation;
        Z = other.Z;
    }

    public CanvasPosition(float[] positionArgs)
    {
        if ((positionArgs != null) && (positionArgs.Length >= 5))
        {
            Coordinates = new Vector2(positionArgs[0], positionArgs[1]);
            ScaleVector = new Vector2(positionArgs[2], positionArgs[3]);
            Rotation = (float)((float)(positionArgs[4] / 180) * Math.PI);

            if (positionArgs.Length >= 6)
            {
                Z = (int)positionArgs[5];
            }
        }
    }

    public CanvasPosition Transform(CanvasPosition transformation)
    {
        ScaleMultiplicative(transformation.ScaleVector);
        Translate(transformation.Coordinates);
        Rotate(transformation.Rotation);
        ZTranslate(transformation.Z);

        return this;
    }

    public CanvasPosition Translate(Vector2 translation)
    {
        Coordinates += translation;

        return this;
    }

    public CanvasPosition ZTranslate(int zTranslation)
    {
        Z += zTranslation;

        return this;
    }

    public CanvasPosition ScaleMultiplicative(Vector2 scale)
    {
        ScaleVector *= scale;

        return this;
    }

    public CanvasPosition ScaleAdditive(Vector2 scale)
    {
        ScaleVector += scale;

        return this;
    }

    public CanvasPosition Rotate(float rotation, Vector2 origin)
    {
        Coordinates = RotatePointAroundPoint(Coordinates, origin, rotation);

        return Rotate(rotation);
    }

    public static Vector2 RotatePointAroundPoint(Vector2 point, Vector2 origin, float rotation)
    {
        Vector2 relativePos = point - origin;

        float newX = (float)((relativePos.X * Math.Cos(rotation)) - (relativePos.Y * Math.Sin(rotation)));
        float newY = (float)((relativePos.Y * Math.Cos(rotation)) + (relativePos.X * Math.Sin(rotation)));

        Vector2 newRelativePos = new Vector2(newX, newY);

        return newRelativePos + origin;
    }

    public CanvasPosition Rotate(float rotation)
    {
        Rotation += rotation;

        return this;
    }

    public CanvasPosition Set(CanvasPosition transformation)
    {
        ScaleVector = transformation.ScaleVector;
        Coordinates = transformation.Coordinates;
        Rotation = transformation.Rotation;
        Z = transformation.Z;

        return this;
    }

    public CanvasPosition Set(Vector2 coordinates)
    {
        Coordinates = coordinates;

        return this;
    }

    public const float MAXROTATION = (float)(Math.PI * 2);

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
    public static CanvasPosition operator +(CanvasPosition a, CanvasPosition b)
       => new CanvasPosition(a.X + b.X, a.Y + b.Y, a.Width * b.Width, a.Height * b.Height, a.Rotation + b.Rotation, a.Z + b.Z);
}

