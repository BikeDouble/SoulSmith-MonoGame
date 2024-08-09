using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

public class CanvasTransformationRule_Rotation : CanvasTransformationRule
{
    private Vector2 _origin;
    private float _baseRotation = 0;
    private float _peakRotationDiff = 0;

    public CanvasTransformationRule_Rotation(
        CanvasItem affectedItem,
        int[] activeStates,
        float rotation,
        float peakRotationDiff,
        Vector2 origin,
        float totalTransformationDuration = -1,
        Func<float, float> velocity = null) : base(affectedItem, activeStates, totalTransformationDuration, velocity)
    {

        _origin = origin;
        _baseRotation = rotation * (float)(Math.PI/180);
        _peakRotationDiff = peakRotationDiff * (float)(Math.PI / 180);
    }

    public CanvasTransformationRule_Rotation(CanvasTransformationRule_Rotation other, CanvasItem affectedItem = null) : base(other, affectedItem)
    {
        _baseRotation = other._baseRotation;
        _peakRotationDiff = other._peakRotationDiff;
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

    protected override void TransformInternal(double delta, float magnitude, CanvasItem item)
    {
        base.TransformInternal(delta, magnitude, item);

        float rotation = (_baseRotation + (_peakRotationDiff * magnitude)) * (float)delta;
        //Debug.Assert(magnitude == 0);

        item.Rotate(rotation, _origin);
    }
}

