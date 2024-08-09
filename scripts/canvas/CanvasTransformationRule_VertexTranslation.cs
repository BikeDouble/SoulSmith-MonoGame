
using Microsoft.Xna.Framework;
using System;

public class CanvasTransformationRule_VertexTranslation : CanvasTransformationRule
{
    private int[] _verticeIndices;
    private Vector2 _translation;
    private Vector2 _peakTranslationDiff;

    public CanvasTransformationRule_VertexTranslation(
        CanvasItem affectedItem,
        int[] activeStates,
        int[] verticeIndices,
        Vector2 translation,
        Vector2 peakTranslationDiff,
        float totalTransformationDuration = -1,
        Func<float, float> velocity = null) : base(affectedItem, activeStates, totalTransformationDuration, velocity)
    {
        _translation = translation;
        _verticeIndices = verticeIndices;
        _peakTranslationDiff = peakTranslationDiff;
    }

    public CanvasTransformationRule_VertexTranslation(CanvasTransformationRule_VertexTranslation other, CanvasItem affectedItem = null) : base(other, affectedItem)
    {
        _verticeIndices = other._verticeIndices;
        _translation = other._translation;
        _peakTranslationDiff = other._peakTranslationDiff;
    }

    public override object DeepClone()
    {
        return new CanvasTransformationRule_VertexTranslation(this);
    }

    public override object DeepClone(CanvasItem affectedItem)
    {
        return new CanvasTransformationRule_VertexTranslation(this, affectedItem);
    }

    protected override void TransformInternal(double delta, float magnitude, CanvasItem item)
    {
        base.TransformInternal(delta, magnitude, item);
        //TODO
        Vector2 translation = _translation + (_peakTranslationDiff * magnitude);

    }
}

