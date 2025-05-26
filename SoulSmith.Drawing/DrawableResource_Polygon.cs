
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public class DrawableResource_Polygon : DrawableResource
    {
        private Polygon _staticPolygon;
        private bool _filled;
        private int _lineThickness;

        public DrawableResource_Polygon(DrawableResource_Polygon other) : base(other)
        {
            _staticPolygon = new Polygon(other._staticPolygon.Vertices);
            _filled = other._filled;
            _lineThickness = other._lineThickness;
        }

        public DrawableResource_Polygon(ColoredPolygon polygon) : base(polygon.Color)
        {
            _staticPolygon = new Polygon(polygon.Polygon.Vertices);
            _filled = polygon.Filled;
            _lineThickness = polygon.LineThickness;
        }

        public DrawableResource_Polygon(Polygon polygon, Color color, bool filled = false, int lineWidth = 5) : base(color)
        {
            _staticPolygon = new Polygon(polygon.Vertices);
            _filled = filled;
            _lineThickness = lineWidth;
        }

        public override void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch)
        {
            Polygon drawnPolygon = _staticPolygon.TransformedCopy(Vector2.Zero, position.Rotation, position.ScaleVector);

            DrawUnfilledInternal(drawnPolygon, position, spriteBatch, GetTintedColor(tint), _lineThickness);
        }

        public static void DrawUnfilledInternal(Polygon polygon, IReadOnlyPosition position, SpriteBatch spriteBatch, Color color, int lineThickness = 1)
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

        public override bool ContainsPoint(Vector2 point, IReadOnlyPosition position)
        {
            Polygon drawnPolygon = _staticPolygon.TransformedCopy(Vector2.Zero, position.Rotation, position.ScaleVector);

            return drawnPolygon.Contains(point);
        }
    }
}
