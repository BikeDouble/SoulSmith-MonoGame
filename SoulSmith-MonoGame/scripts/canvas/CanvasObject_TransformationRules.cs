using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using SoulSmith.Drawing;
using SoulSmith.Core;

public class CanvasObject_TransformationRules : CanvasObject, IReadOnlyCanvasItem_TransformationRules
{
    private List<CanvasTransformationRule> _transformationRules;
    private int _state = -1;
    private double _transformationCoef = 1;

    public CanvasObject_TransformationRules(
        List<CanvasTransformationRule> rules, 
        List<SoulSmithObject> unrulyChildren = null, 
        Dictionary<BoundingZoneType, CanvasObject> boundingZones = null,
        Position position = null) : base(position, null, boundingZones, unrulyChildren)
    {
        _transformationRules = rules;

        foreach (CanvasTransformationRule rule in rules)
        {
            AddChild(rule);
        }
    }

    public CanvasObject_TransformationRules() { }

    public CanvasObject_TransformationRules(CanvasObject_TransformationRules other, CanvasObject shelledItem = null, double transformationCoef = 1) : base(other, shelledItem)
    {
        _transformationCoef = transformationCoef;

        _transformationRules = CloneTransformationRules(
            other._transformationRules,
            Children,
            other.Children,
            shelledItem);
    }

    public CanvasObject_TransformationRules(CanvasObject other, CanvasObject shelledItem = null, double transformationCoef = 1) : base(other, shelledItem)
    {
        _transformationCoef = transformationCoef;

       CanvasObject_TransformationRules otherAsTransformationRules = other as CanvasObject_TransformationRules;

        if (otherAsTransformationRules != null)
        {
            _transformationRules = CloneTransformationRules(
                otherAsTransformationRules._transformationRules,
                Children,
                otherAsTransformationRules.Children,
                shelledItem);
        }
        else
        {
            _transformationRules = null;
        }
    }

    private static List<CanvasTransformationRule> CloneTransformationRules(
        List<CanvasTransformationRule> otherRules,
        ReadOnlyCollection<SoulSmithObject> children,
        ReadOnlyCollection<SoulSmithObject> otherChildren,
        CanvasObject shelledItem)
    {
        if (otherRules == null) return null;

        List<CanvasTransformationRule> rules = new();

        foreach (CanvasTransformationRule rule in otherRules)
        {
            int ruleIndex = otherChildren.IndexOf(rule.AffectedItem as SoulSmithObject);

            CanvasObject affectedItem = (ruleIndex == -1) ? null : children[ruleIndex] as CanvasObject;

            Debug.Assert((affectedItem != null) || (shelledItem != null));

            if (affectedItem != null)
            {
                CanvasTransformationRule newRule = (CanvasTransformationRule)rule.DeepClone(affectedItem);
                rules.Add(newRule);
            }
            else
            {
                CanvasTransformationRule newRule = (CanvasTransformationRule)rule.DeepClone(shelledItem);
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
            double tDelta = delta * _transformationCoef;

            foreach (CanvasTransformationRule rule in _transformationRules)
            {
                rule.Transform(tDelta);
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
            if (child is CanvasObject_TransformationRules)
                ((CanvasObject_TransformationRules)child).UpdateState(newState);
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
        CanvasObject newChild = rule.AffectedItem as CanvasObject;

        AddChild(newChild);

        if (!_transformationRules.Contains(rule))
            _transformationRules.Add(rule);
    }

    public override object DeepClone()
    {
        return new CanvasObject_TransformationRules(this);
    }

    public object DeepClone(CanvasObject shelledItem)
    {
        return new CanvasObject_TransformationRules(this, shelledItem);
    }

    public ReadOnlyCollection<CanvasTransformationRule> GetAllTransformationRulesInChildTreeWithTag(string tag)
    {
        tag = tag.ToLower();

        ReadOnlyCollection<SoulSmithObject> children = Children;

        List<CanvasTransformationRule> rules = new List<CanvasTransformationRule>();

        if (_transformationRules != null)
        {
            foreach (CanvasTransformationRule rule in _transformationRules)
            {
                if (rule.Tag == tag)
                    rules.Add(rule);
            }
        }

        foreach (SoulSmithObject child in children)
        {
            CanvasObject_TransformationRules ruleChild = child as CanvasObject_TransformationRules;

            if (ruleChild != null)
            {
                rules.AddRange(ruleChild.GetAllTransformationRulesInChildTreeWithTag(tag));
            }
        }

        return rules.AsReadOnly();
    }

    public int State { get { return _state; } }
    protected double TransformationCoef { get { return _transformationCoef; } set { _transformationCoef = value; } }
}

