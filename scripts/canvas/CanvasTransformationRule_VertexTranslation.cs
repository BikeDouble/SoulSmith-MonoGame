
using Microsoft.Xna.Framework;

public class CanvasTransformationRule_VertexTranslation : CanvasTransformationRule
{
    private int[] _verticeIndices;
    private Vector2 _translation;

    public CanvasTransformationRule_VertexTranslation(
        CanvasItem affectedItem,
        int[] activeStates,
        int[] verticeIndices,
        Vector2 translation,
        float totalTransformationDuration = -1,
        float acceleration = 1) : base(affectedItem, activeStates, totalTransformationDuration, acceleration)
    {
        _translation = translation;
        _verticeIndices = verticeIndices;
    }

    public CanvasTransformationRule_VertexTranslation(CanvasTransformationRule_VertexTranslation other, CanvasItem affectedItem = null) : base(other, affectedItem)
    {
        _verticeIndices = other._verticeIndices;
        _translation = new Vector2(other._translation.X, other._translation.Y);
    }

    public override object DeepClone()
    {
        return new CanvasTransformationRule_VertexTranslation(this);
    }

    public override object DeepClone(CanvasItem affectedItem)
    {
        return new CanvasTransformationRule_VertexTranslation(this, affectedItem);
    }

    protected override void TransformInternal(double delta, CanvasItem item)
    {
        base.TransformInternal(delta, item);
        //TODO

    }
}

