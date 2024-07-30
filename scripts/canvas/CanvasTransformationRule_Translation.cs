using Microsoft.Xna.Framework;

public class CanvasTransformationRule_Translation : CanvasTransformationRule
{
    private Vector2 _translation;

    public CanvasTransformationRule_Translation(
        CanvasItem affectedItem,
        int[] activeStates,
        Vector2 translation,
        float totalTransformationDuration = -1,
        float acceleration = 1) : base(affectedItem, activeStates, totalTransformationDuration, acceleration)
    {
        _translation = translation;
    }

    public CanvasTransformationRule_Translation(CanvasTransformationRule_Translation other, CanvasItem affectedItem = null) : base(other, affectedItem)
    {
        _translation = new Vector2(other._translation.X, other._translation.Y);
    }

    public override object DeepClone()
    {
        return new CanvasTransformationRule_Translation(this);
    }

    public override object DeepClone(CanvasItem affectedItem)
    {
        return new CanvasTransformationRule_Translation(this, affectedItem);
    }

    protected override void TransformInternal(double delta, CanvasItem item)
    {
        base.TransformInternal(delta, item);

        item.Translate(_translation * (float)delta);
    }
}

