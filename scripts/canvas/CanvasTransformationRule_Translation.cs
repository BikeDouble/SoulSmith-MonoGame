using Microsoft.Xna.Framework;
using System;

public class CanvasTransformationRule_Translation : CanvasTransformationRule
{
    private Vector2 _translation;
    private Vector2 _peakTranslationDiff;

    public CanvasTransformationRule_Translation(
        CanvasItem affectedItem,
        int[] activeStates,
        Vector2 translation,
        Vector2 peakTranslationDiff,
        float totalTransformationDuration = -1,
        Func<float, float> velocity = null) : base(affectedItem, activeStates, totalTransformationDuration, velocity)
    {
        _translation = translation;
    }

    public CanvasTransformationRule_Translation(CanvasTransformationRule_Translation other, CanvasItem affectedItem = null) : base(other, affectedItem)
    {
        _translation = other._translation;
        _peakTranslationDiff = other._peakTranslationDiff;
    }

    public override object DeepClone()
    {
        return new CanvasTransformationRule_Translation(this);
    }

    public override object DeepClone(CanvasItem affectedItem)
    {
        return new CanvasTransformationRule_Translation(this, affectedItem);
    }

    protected override void TransformInternal(double delta, float magnitude, CanvasItem item)
    {
        base.TransformInternal(delta, magnitude, item);

        Vector2 translation = _translation + (_peakTranslationDiff * magnitude);

        item.Translate(translation * (float)delta);
    }
}

