using Microsoft.Xna.Framework;
using System;

public class CanvasTransformationRule_Rotation : CanvasTransformationRule
{
    private Vector2 _origin;
    private float _rotation;

    public CanvasTransformationRule_Rotation(
        CanvasItem affectedItem,
        int[] activeStates,
        float rotation,
        Vector2 origin,
        float totalTransformationDuration = -1,
        float acceleration = 1) : base(affectedItem, activeStates, totalTransformationDuration, acceleration)
    {
        _origin = origin;
        _rotation = rotation * (float)(Math.PI/180);
    }

    public CanvasTransformationRule_Rotation(CanvasTransformationRule_Rotation other, CanvasItem affectedItem = null) : base(other, affectedItem)
    {
        _rotation = other._rotation;
        _origin = new Vector2(other._origin.X, other._origin.Y);
    }

    public override object DeepClone()
    {
        return new CanvasTransformationRule_Rotation(this);
    }

    public override object DeepClone(CanvasItem affectedItem)
    {
        return new CanvasTransformationRule_Rotation(this, affectedItem);
    }

    protected override void TransformInternal(double delta, CanvasItem item)
    {
        base.TransformInternal(delta, item);

        item.Rotate(_rotation * (float)delta, _origin);
    }
}

