using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using SoulSmith.Drawing;
using SoulSmith.Core;

public class BoundingZone_Ellipse : BoundingZone
{
    private float _radius = 0;

    public BoundingZone_Ellipse(BoundingZone_Ellipse other) 
    {
        _radius = other._radius;
    }

    public BoundingZone_Ellipse(float radius, bool showOutline = true, Position position = null) : base(showOutline, position)
    {
        _radius = radius;

        if (showOutline)
        {
            Resource = new DrawableResource_Polygon(ColoredPolygon.RegularPolygon(radius, 20), Color.Red);
        }
    }

    public override Vector2 GetRandomBoundingPointLocal(BoundingZoneType zoneType = BoundingZoneType.None)
    {
        float direction = Rand.RandFloat() * SoulSmith.Core.Position.MAXROTATION;

        float distance = (float)(Math.Sqrt(Rand.RandFloat()) * _radius);

        Vector2 point = new Vector2(distance, 0);

        point = SoulSmith.Core.Position.RotatePointAroundPoint(point, Vector2.Zero, direction);

        point *= Position.ScaleVector;

        point = SoulSmith.Core.Position.RotatePointAroundPoint(point, Vector2.Zero, Position.Rotation);

        return point;
    }

    public override Vector2 GetRandomBoundingPointGlobal(BoundingZoneType zoneType = BoundingZoneType.None)
    {
        Vector2 globalCoordinates = GetGlobalPosition().Coordinates;

        return (GetRandomBoundingPointLocal() + globalCoordinates);
    }

    public override object DeepClone()
    {
        return new BoundingZone_Ellipse(this);
    }
}

