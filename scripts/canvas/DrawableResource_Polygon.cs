
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

public class DrawableResource_Polygon : DrawableResource
{
    private Color _color;
    private Polygon _staticPolygon;
    private bool _filled;
    private int _lineThickness;

    public DrawableResource_Polygon(DrawableResource_Polygon other) : base(other)
    {
        _staticPolygon = new Polygon(other._staticPolygon.Vertices);
        _color = other._color;
        _filled = other._filled;
        _lineThickness = other._lineThickness;
    }

    public DrawableResource_Polygon(TrackedResource<ColoredPolygon> polygon)
    {
        _staticPolygon = new Polygon(polygon.Resource.Polygon.Vertices);
        _color = polygon.Resource.Color;
        _filled = polygon.Resource.Filled;
        _lineThickness = polygon.Resource.LineThickness;
    }

    public DrawableResource_Polygon(Polygon polygon, Color color, bool filled = false, int lineWidth = 5)
    {
        _staticPolygon = new Polygon(polygon.Vertices);
        _color = color;
        _filled = filled;
        _lineThickness = lineWidth;
    }

    public override void UpdateColor(Color color)
    {
        _color = color;

        base.UpdateColor(color);
    }

    public override void Draw(Position position, SpriteBatch spriteBatch)
    {
        Polygon drawnPolygon = _staticPolygon.TransformedCopy(Vector2.Zero, position.Rotation, position.Scale);

        DrawUnfilledInternal(drawnPolygon, position, spriteBatch, _color, _lineThickness);
    }

    public static void DrawUnfilledInternal(Polygon polygon, Position position, SpriteBatch spriteBatch, Color color, int lineThickness = 1)
    { 
        spriteBatch.DrawPolygon(
                position.Coordinates,
                polygon,
                color,
                lineThickness);
    }

    public override object DeepClone()
    {
        return new DrawableResource_Polygon(this);
    }

    public override bool ContainsPoint(Vector2 point, Position position)
    {
        Polygon drawnPolygon = _staticPolygon.TransformedCopy(Vector2.Zero, position.Rotation, position.Scale);

        return drawnPolygon.Contains(point);
    }
}

