
using System.Linq;

public class CanvasTransformationRule : IDeepCloneable
{
    private CanvasItem _affectedItem;

    private int[] _activeStates;
    private bool _bActive = true;
    private float _totalDuration = -1;
    private float _elapsedDuration = 0;
    private float _acceleration = 1;

    public CanvasTransformationRule(
        CanvasItem affectedItem,
        int[] activeStates,
        float totalTransformationDuration = -1,
        float acceleration = 1)
    {
        _affectedItem = affectedItem;
        _activeStates = activeStates;
        _totalDuration = totalTransformationDuration;
        _acceleration = acceleration;
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
        _acceleration = other._acceleration;   
        _bActive = other._bActive;
    }

    public void UpdateState(int newState)
    {
        _bActive = _activeStates.Contains(newState);
    }

    public void Transform(double delta)
    {
        if ((_affectedItem != null) && (_bActive))
        {
            IncrementDuration(delta);

            double passedDelta = delta;

            if (_totalDuration > 0)
            {
                passedDelta /= _totalDuration;
            }

            TransformInternal(passedDelta, _affectedItem);

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

    protected virtual void TransformInternal(double delta, CanvasItem item)
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

    public CanvasItem AffectedItem { get { return _affectedItem; } }
}

