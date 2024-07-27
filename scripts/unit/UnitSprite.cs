
using System;
using Microsoft.Xna.Framework;

public partial class UnitSprite : CanvasItem_TransformationRules
{
    private BoundingZone _sendingZone = null;
    private BoundingZone _receivingZone = null;

    public UnitSprite(UnitSprite other) : base(other)
    {
        _sendingZone = other._sendingZone;
        _receivingZone = other._receivingZone;
    }

    public UnitSprite(CanvasItem_TransformationRules other) : base(other)
    {

    }

    public Vector2 GetRandomVisualizationPoint()
    {
        return Vector2.Zero;
        return _sendingZone.GetRandomPoint();
    }
    
}
