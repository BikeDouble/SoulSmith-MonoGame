using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

public class CanvasItem_TransformationRules : CanvasItem
{
    private List<CanvasTransformationRule> _transformationRules;

    public CanvasItem_TransformationRules(
        List<CanvasTransformationRule> rules, 
        List<FormsObject> unrulyChildren = null, 
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = null) : base(null, boundingZones)
    {
        _transformationRules = rules;

        foreach (CanvasTransformationRule rule in rules)
        {
            AddChild(rule);
        }

        if (unrulyChildren != null)
        {
            foreach (FormsObject child in unrulyChildren)
            {
                AddChild(child);
            }
        }
    }

    public CanvasItem_TransformationRules(CanvasItem_TransformationRules other) : base(other)
    {
        _transformationRules = CloneTransformationRules(
            other._transformationRules,
            Children,
            other.Children);
    }

    private static List<CanvasTransformationRule> CloneTransformationRules(
        List<CanvasTransformationRule> otherRules,
        ReadOnlyCollection<FormsObject> children,
        ReadOnlyCollection<FormsObject> otherChildren)
    {
        if (otherRules == null) return null;

        List<CanvasTransformationRule> rules = new();

        foreach (CanvasTransformationRule rule in otherRules)
        {
            int ruleIndex = otherChildren.IndexOf(rule.AffectedItem);

            CanvasItem affectedItem = children[ruleIndex] as CanvasItem;

            Debug.Assert(affectedItem != null);

            if (affectedItem != null)
            {
                CanvasTransformationRule newRule = (CanvasTransformationRule)rule.DeepClone(affectedItem);
                rules.Add(newRule);
            }
        }

        if (rules.Count < 1) return null;

        return rules;
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

        AddChild(newChild);

        if (!_transformationRules.Contains(rule))
            _transformationRules.Add(rule);
    }

    public override object DeepClone()
    {
        return new CanvasItem_TransformationRules(this);
    }
}

