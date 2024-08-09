using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

public class CanvasItem_TransformationRules : CanvasItem
{
    private List<CanvasTransformationRule> _transformationRules;
    private int _state = -1;

    public CanvasItem_TransformationRules(
        List<CanvasTransformationRule> rules, 
        List<SoulSmithObject> unrulyChildren = null, 
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = null,
        Position position = null) : base(position, null, boundingZones, unrulyChildren)
    {
        _transformationRules = rules;

        foreach (CanvasTransformationRule rule in rules)
        {
            AddChild(rule);
        }
    }

    public CanvasItem_TransformationRules() { }

    public CanvasItem_TransformationRules(CanvasItem_TransformationRules other) : base(other)
    {
        _transformationRules = CloneTransformationRules(
            other._transformationRules,
            Children,
            other.Children);
    }

    private static List<CanvasTransformationRule> CloneTransformationRules(
        List<CanvasTransformationRule> otherRules,
        ReadOnlyCollection<SoulSmithObject> children,
        ReadOnlyCollection<SoulSmithObject> otherChildren)
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
        if (_transformationRules != null)
        {
            foreach (CanvasTransformationRule rule in _transformationRules)
            {
                rule.Transform(delta);
            }
        }

        base.Process(delta);
    }

    public void UpdateState(int newState)
    {
        if (_state == newState) return;

        _state = newState;

        if (_transformationRules != null)
        {
            foreach (CanvasTransformationRule rule in _transformationRules)
            {
                rule.UpdateState(newState);
            }
        }

        foreach (SoulSmithObject child in Children)
        {
            if (child is CanvasItem_TransformationRules)
                ((CanvasItem_TransformationRules)child).UpdateState(newState);
        }
    }

    public override void RemoveChild(SoulSmithObject child)
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

    public int State { get { return _state; } }
}

