using System;
using Microsoft.Xna.Framework;

public class BoundingZone : CanvasItem
{
    private bool _showOutline;

    public BoundingZone(BoundingZone other) : base(other)
    {
        _showOutline = other._showOutline;
    }

    public BoundingZone(bool showOutline = false, Position position = null) : base(position)
    {
        _showOutline = showOutline;
    }

    public override Vector2 GetRandomBoundingPointLocal(BoundingZoneType zoneType = BoundingZoneType.None)
    {  
        return Vector2.Zero;
    }

    public override Vector2 GetRandomBoundingPointGlobal(BoundingZoneType zoneType = BoundingZoneType.None)
    {
        return GetGlobalPosition().Coordinates;
    }

    public override object DeepClone()
    {
        return new BoundingZone(this);
    }

    public bool ShowOutline { get { return _showOutline; } }
}
