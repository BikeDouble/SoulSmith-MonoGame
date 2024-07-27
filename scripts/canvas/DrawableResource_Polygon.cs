
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;

public class DrawableResource_Polygon : DrawableResource
{
    private Color _color;
    private Polygon _staticPolygon;
    private Polygon _drawnPolygon;

    public DrawableResource_Polygon(DrawableResource_Polygon other) : base(other)
    {
        _staticPolygon = new Polygon(other._staticPolygon.Vertices);
        _color = other._color;
        _drawnPolygon = _staticPolygon;
    }

    public DrawableResource_Polygon(TrackedResource<ColoredPolygon> polygon)
    {
        _staticPolygon = new Polygon(polygon.Resource.Polygon.Vertices);
        _drawnPolygon = _staticPolygon;
        _color = polygon.Resource.Color;
    }

    public DrawableResource_Polygon(Polygon polygon, Color color)
    {
        _staticPolygon = new Polygon(polygon.Vertices);
        _drawnPolygon = _staticPolygon;
        _color = color;
    }

    public override void UpdateColor(Color color)
    {
        _color = color;

        base.UpdateColor(color);
    }

    public override void UpdatePosition(Position position)
    {
        _drawnPolygon = ColoredPolygon.DeepCopy(_staticPolygon);
        _drawnPolygon.Scale(position.ScaleAsVector2());
        _drawnPolygon.Rotate(position.Rotation);

        base.UpdatePosition(position);
    }

    public override void Draw(Position position, SpriteBatch spriteBatch)
    {
        DrawInternal(_drawnPolygon, position, spriteBatch, _color);
    }

    public static void DrawInternal(Polygon polygon, Position position, SpriteBatch spriteBatch, Color color)
    {
        spriteBatch.DrawPolygon(
                position.Coordinates,
                polygon,
                color,
                5);
    }

    public override object DeepClone()
    {
        return new DrawableResource_Polygon(this);
    }

    public override bool ContainsPoint(Vector2 point, Position position)
    {
        return _drawnPolygon.Contains(point);
    }
}

