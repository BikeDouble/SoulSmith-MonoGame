using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

public class BoundingZone_Ellipse : BoundingZone
{
    private float _radius = 0;
    private bool _showOutline;

    public BoundingZone_Ellipse(float radius, bool showOutline = true)
    {
        _radius = radius;

        if (showOutline)
        {
            Resource = new DrawableResource_Polygon(AssetLoader.RegularPolygon(radius, 20), Color.Red);
        }
    }

    public override void Draw(Position absolutePosition, SpriteBatch spriteBatch)
    {
        base.Draw(absolutePosition, spriteBatch);
    }

    public override Vector2 GetRandomPoint()
    {
        float direction = Rand.RandFloat() * Position.MAXROTATION;

        float distance = Rand.RandFloat() * _radius;

        Vector2 point = new Vector2(distance, 0);

        point = Position.RotatePointAroundPoint(point, Vector2.Zero, direction);

        point *= Position.Scale;

        point = Position.RotatePointAroundPoint(point, Vector2.Zero, Position.Rotation);

        return point;
    }
}

