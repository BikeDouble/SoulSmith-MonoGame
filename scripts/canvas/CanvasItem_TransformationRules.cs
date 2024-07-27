using System.Collections.Generic;
using System.Data;

public class CanvasItem_TransformationRules : CanvasItem
{
    private List<CanvasTransformationRule> _transformationRules;

    public CanvasItem_TransformationRules(float[] positionArgs) : base(positionArgs)
    {

    }

    public CanvasItem_TransformationRules(List<CanvasTransformationRule> rules) 
    {
        _transformationRules = rules;

        AddAllChildrenInTransformationRules();
    }

    public CanvasItem_TransformationRules(List<CanvasTransformationRule> rules, List<FormsObject> unrulyChildren)
    {
        _transformationRules = rules;

        AddAllChildrenInTransformationRules();

        foreach (FormsObject child in unrulyChildren)
        {
            AddChild(child);
        }
    }

    public CanvasItem_TransformationRules(CanvasItem_TransformationRules other) : base(other)
    { 
        _transformationRules = new List<CanvasTransformationRule>();

        foreach (CanvasTransformationRule rule in other._transformationRules)
        {
            CanvasTransformationRule newRule = null;

            if (rule is CanvasTransformationRule_Rotation)
            {
                newRule = new CanvasTransformationRule_Rotation((CanvasTransformationRule_Rotation)rule);
            }
            else if (rule is CanvasTransformationRule_Translation)
            {
                newRule = new CanvasTransformationRule_Translation((CanvasTransformationRule_Translation)rule);
            }
            else if (rule is CanvasTransformationRule_VertexTranslation)
            {
                newRule = new CanvasTransformationRule_VertexTranslation((CanvasTransformationRule_VertexTranslation)rule);
            }

            if (newRule != null)
            {
                _transformationRules.Add(newRule);
            }
            else if (rule.AffectedItem != null)
            {
                AddChild(rule.AffectedItem);
            }
        }

        AddAllChildrenInTransformationRules();
    }

    private void AddAllChildrenInTransformationRules()
    {
        foreach (CanvasTransformationRule rule in _transformationRules)
        {
            AddChild(rule);
        }
    }

    public override void Process(double delta)
    {
        foreach (CanvasTransformationRule rule in _transformationRules)
        {
            rule.Transform(delta);
        }

        base.Process(delta);
    }

    public override void RemoveChild(FormsObject child)
    {
        foreach (CanvasTransformationRule rule in _transformationRules)
        {
            if (rule.AffectedItem == child)
            {
                _transformationRules.Remove(rule);
            }
        }

        base.RemoveChild(child);
    }

    public void AddChild(CanvasTransformationRule rule) 
    {
        CanvasItem newChild = rule.AffectedItem;

        base.AddChild(newChild);

        if (!_transformationRules.Contains(rule))
            _transformationRules.Add(rule);
    }

    public override object DeepClone()
    {
        return new CanvasItem_TransformationRules(this);
    }
}

