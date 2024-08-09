
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

public class CanvasTransformationRule : IDeepCloneable
{
    private CanvasItem _affectedItem;

    private int[] _activeStates;
    private bool _bActive = false;
    private float _totalDuration = -1;
    private float _elapsedDuration = -1;
    private Func<float, float> _velocity = null;

    public CanvasTransformationRule(
        CanvasItem affectedItem,
        int[] activeStates,
        float totalTransformationDuration = -1,
        Func<float, float> velocity = null)
    {
        _affectedItem = affectedItem;
        _activeStates = activeStates;
        _totalDuration = totalTransformationDuration;
        _velocity = velocity;
    }

    public CanvasTransformationRule(CanvasTransformationRule other, CanvasItem affectedItem = null)
    {
        if (affectedItem == null)
        {
            _affectedItem = (CanvasItem)other._affectedItem.DeepClone();
        }
        else
        {
            _affectedItem = affectedItem;
        }

        _activeStates = other._activeStates;
        _totalDuration = other._totalDuration;
        _bActive = other._bActive;
        _velocity = other._velocity;
    }

    public void UpdateState(int newState)
    {
        _bActive = _activeStates.Contains(newState);
        _elapsedDuration = 0;
    }

    public void Transform(double delta)
    {
        if ((_affectedItem != null) && (_bActive))
        {
            IncrementDuration(delta);

            float magnitude = 0;

            if (_velocity != null)
            {
                if (_totalDuration > 0)
                {
                    magnitude = _velocity(_elapsedDuration / _totalDuration);
                }
                else
                {
                    magnitude = _velocity((float)delta);
                }
            }

            TransformInternal(delta, magnitude, _affectedItem);

            _bActive = !ExcededDuration();
        }
    }

    private float IncrementDuration(double delta)
    {
        _elapsedDuration += (float)delta;

        if (_elapsedDuration > _totalDuration)
        {
            _elapsedDuration = _totalDuration;
        }

        return _elapsedDuration;
    }

    private bool ExcededDuration()
    {
        if (_totalDuration <= 0)
        {
            return false;
        }

        if (_elapsedDuration >= _totalDuration)
        {
            _elapsedDuration = 0;
            return true;
        }

        return false;
    }

    protected virtual void TransformInternal(double delta, float magnitude, CanvasItem item)
    {

    }

    public virtual object DeepClone()
    {
        return new CanvasTransformationRule(this);
    }

    public virtual object DeepClone(CanvasItem affectedItem)
    {
        return new CanvasTransformationRule(this, affectedItem);
    }

    public static Func<float, float> SmoothBell = (time) =>
    {
        float x = 2.828f * (time - 0.5f);
        float term1 = (float)Math.Pow(x, 4) / 4;
        float term2 = -(float)Math.Pow(x, 2);
        float y = term1 + term2 + 1;
        return y;
    };

    public CanvasItem AffectedItem { get { return _affectedItem; } }
}

