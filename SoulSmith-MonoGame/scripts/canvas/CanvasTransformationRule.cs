
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using SoulSmith.Drawing;
using SoulSmith.Core;

public class CanvasTransformationRule : IDeepCloneable
{
    private readonly ITransformable _affectedItem;

    private readonly string _tag = string.Empty;
    private readonly int[] _activeStates;
    private bool _bActive = false;
    private readonly bool _bPeakIsTarget;
    private readonly float _totalDuration = -1;
    private readonly float _cycleDuration = -1;
    private float _elapsedDuration = -1;
    private float _elapsedCycleDuration = -1;
    private float _accumulatedMagnitude = 0;
    private readonly Func<float, ReadOnlyCollection<float>, float> _velocityDelegate = null;
    private readonly Action<TransformaionRuleDelegateArgs, ITransformable, double, float> _transformDelegate = null;
    private ReadOnlyCollection<float> _velocityFuncArgs = null;
    private ReadOnlyCollection<float> _pendingVelocityFuncArgs = null;
    private readonly TransformaionRuleDelegateArgs _transformDelegateArgs = null;

    public CanvasTransformationRule(
        ITransformable affectedItem,
        int[] activeStates,
        TransformaionRuleDelegateArgs transformDelegateArgs,
        Action<TransformaionRuleDelegateArgs, ITransformable, double, float> transformDelegate,
        float totalTransformationDuration = -1,
        Func<float, ReadOnlyCollection<float>, float> velocity = null,
        bool peakIsTarget = false,
        float cycleDuration = -1,
        string tag = null)
    {
        _affectedItem = affectedItem;
        _activeStates = activeStates;
        _totalDuration = totalTransformationDuration;
        _velocityDelegate = velocity;
        _bPeakIsTarget = peakIsTarget;
        _cycleDuration = cycleDuration;
        _transformDelegateArgs = transformDelegateArgs;
        _transformDelegate = transformDelegate;
        _tag = tag;
        if (_tag == null) _tag = string.Empty;
    }

    public CanvasTransformationRule(CanvasTransformationRule other, CanvasItem affectedItem = null)
    {
        if (affectedItem == null)
        {
            _affectedItem = (ITransformable)other._affectedItem.DeepClone();
        }
        else
        {
            _affectedItem = affectedItem;
        }

        _activeStates = other._activeStates;
        _totalDuration = other._totalDuration;
        _bActive = other._bActive;
        _velocityDelegate = other._velocityDelegate;
        _bPeakIsTarget = other._bPeakIsTarget;
        _cycleDuration = other._cycleDuration;
        _transformDelegate = other._transformDelegate;
        _velocityFuncArgs = other._velocityFuncArgs;
        _pendingVelocityFuncArgs = other._pendingVelocityFuncArgs;
        _transformDelegateArgs = other._transformDelegateArgs;
    }

    public void UpdateState(int newState)
    {
        bool previouslyActive = _bActive;
        _bActive = _activeStates.Contains(newState);
        if (_bActive && !previouslyActive)
        {
            _elapsedDuration = 0;
            _elapsedCycleDuration = 0;
        }
    }

    public void SetVelocityFuncArgs(IEnumerable<float> args)
    {
        _pendingVelocityFuncArgs = args.ToList().AsReadOnly();
    }

    private void UpdateVelocityFuncArgs()
    {
        if (_pendingVelocityFuncArgs != null)
        {
            _velocityFuncArgs = _pendingVelocityFuncArgs;
            _pendingVelocityFuncArgs = null;
        }
    }

    public void Transform(double delta)
    {
        if ((_affectedItem != null) && (_bActive))
        {
            if (_totalDuration > 0)
                IncrementDuration(delta);

            if (_cycleDuration > 0)
                IncrementCycleDuration(delta);

            float magnitude = GetMagnitude(delta);

            TransformInternal(delta, magnitude);

            _bActive = !ExcededDuration();
        }
    }

    private float GetMagnitude(double delta)
    {
        float magnitude = 0;

        if (_velocityDelegate != null)
        {
            if (_cycleDuration > 0)
            {
                magnitude = _velocityDelegate(_elapsedCycleDuration / _cycleDuration, _velocityFuncArgs);
            }
            else if (_totalDuration > 0)
            {
                magnitude = _velocityDelegate(_elapsedDuration / _totalDuration, _velocityFuncArgs);
            }

            if (_bPeakIsTarget)
            {
                magnitude -= (float)_accumulatedMagnitude;

                _accumulatedMagnitude += magnitude;

                if (delta != 0)
                    magnitude /= (float)delta;
            }
        }

        return magnitude;
    }

    private void IncrementDuration(double delta)
    {
        _elapsedDuration += (float)delta;

        if (_elapsedDuration > _totalDuration)
        {
            _elapsedDuration = _totalDuration;
        }
    }

    private void IncrementCycleDuration(double delta)
    {
        _elapsedCycleDuration += (float)delta;

        if (_elapsedCycleDuration > _cycleDuration)
        {
            _elapsedCycleDuration -= _cycleDuration;
        }
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

    private void TransformInternal(double delta, float magnitude)
    {
        _transformDelegate(_transformDelegateArgs, _affectedItem, delta, magnitude);
    }

    public virtual object DeepClone()
    {
        return new CanvasTransformationRule(this);
    }

    public virtual object DeepClone(CanvasItem affectedItem)
    {
        return new CanvasTransformationRule(this, affectedItem);
    }

    public static Func<float, ReadOnlyCollection<float>, float> SmoothBell = (time, args) =>
    {
        float x = 2.828f * (time - 0.5f);
        float term1 = (float)Math.Pow(x, 4) / 4;
        float term2 = -(float)Math.Pow(x, 2);
        float y = term1 + term2 + 1;

        return y;
    };

    public static Func<float, ReadOnlyCollection<float>, float> Slope = (time, args) =>
    {
        return time;
    };

    public static Func<float, ReadOnlyCollection<float>, float> SlopePartwayPeak = (time, args) =>
    {
        float strongTime;

        if ((args != null) && (0 > args[0]))
        {
            float prePeakCoef = 1 / args[0];
            strongTime = time * prePeakCoef;
        }
        else
        {
            strongTime = time * 2;
        }

        if (strongTime > 1)
        {
            return 1f;
        }

        return strongTime;
    };

    public static Func<float, ReadOnlyCollection<float>, float> Sin = (time, args) =>
    {
        return (float)Math.Sin(time * 2 * Math.PI);
    };

    public static Func<float, ReadOnlyCollection<float>, float> FormFloat = (time, args) =>
    {
        float health;

        if (args == null)
        {
            health = 1f;
        }
        else
        {
            health = args[0];
        }

        return BetweenTwoFunc(HealthyFormFloat, WoundedFormFloat, health, time, args);
    };

    public static Func<float, ReadOnlyCollection<float>, float> HealthyFormFloat = (time, args) =>
    {
        return (float)Math.Sin(time * 2 * Math.PI);
    };

    private static ReadOnlyCollection<Vector2> WoundedFormFloatPoints = new List<Vector2> 
    {
        new Vector2(0,0),
        new Vector2(0.25f,-0.4f),
        new Vector2(0.49f, -0.5f),
        new Vector2(0.51f, 1.2f),
        new Vector2(0.95f, 1.2f),
        new Vector2(1,0)
    }.AsReadOnly();

    public static Func<float, ReadOnlyCollection<float>, float> WoundedFormFloat = (time, args) =>
    {
        return ConnectedPointsFunc(time, WoundedFormFloatPoints);
    };

    public static Func<
        Func<float, ReadOnlyCollection<float>, float>,
        Func<float, ReadOnlyCollection<float>, float>,
        float,
        float,
        ReadOnlyCollection<float>,
        float>
    BetweenTwoFunc = (func1, func2, func1Fraction, time, args) =>
    {
        float output1 = func1(time, args) * func1Fraction;
        float output2 = func2(time, args) * (1 - func1Fraction);

        return (output1 + output2);
    };

    /// <summary>
    /// Must have points at x = 0 and x = 1
    /// </summary>
    public static Func<float, ReadOnlyCollection<Vector2>, float> ConnectedPointsFunc = (time, points) =>
    {
        Vector2 point1 = Vector2.Zero;
        Vector2 point2 = Vector2.Zero;

        for (int i = 1; i < points.Count; i++)
        {
            if (points[i].X >= time)
            {
                point2 = points[i];
                point1 = points[i - 1];
                break;
            }

            if (i == points.Count - 1)
                return 0;
        };

        float dist = (time - point1.X) / (point2.X - point1.X);
        float ret = ((1 - dist) * point1.Y) + (dist * point2.Y);
        return ret;
    };

    public static Action<TransformaionRuleDelegateArgs, ITransformable, double, float> RotationDelegate = (args, item, delta, magnitude) =>
    {
        float baseRotation = args.Transformation[0];
        float peakRotationDiff = args.PeakTransformationDiff[0];
        Vector2 origin = args.Origin;

        float rotation = (baseRotation + (peakRotationDiff * magnitude)) * (float)delta;

        item.Rotate(rotation, origin);
    };

    public static Action<TransformaionRuleDelegateArgs, ITransformable, double, float> ScaleAdditiveDelegate = (args, item, delta, magnitude) =>
    {
        Vector2 scale = new Vector2(args.Transformation[0], args.Transformation[1]);
        Vector2 peakScaleDiff = new Vector2(args.PeakTransformationDiff[0], args.PeakTransformationDiff[1]);

        Vector2 scaleDiff = (scale + (peakScaleDiff * magnitude)) - Vector2.One;
        scaleDiff = ((float)delta * scaleDiff);

        item.ScaleAdditive(scaleDiff);
    };

    public static Action<TransformaionRuleDelegateArgs, ITransformable, double, float> TranslationDelegate = (args, item, delta, magnitude) =>
    {
        Vector2 translation = new Vector2(args.Transformation[0], args.Transformation[1]);
        Vector2 peakTranslationDiff = new Vector2(args.PeakTransformationDiff[0], args.PeakTransformationDiff[1]);

        translation = (float)delta * (translation + (peakTranslationDiff * magnitude));

        item.Translate(translation);
    };

    public static Action<TransformaionRuleDelegateArgs, ITransformable, double, float> VertexTranslationDelegate = (args, item, delta, magnitude) =>
    {
        
    };

    public static Action<TransformaionRuleDelegateArgs, ITransformable, double, float> AdditiveTintChangeDelegate = (args, item, delta, magnitude) =>
    {
        Vector4 colorChange = new Vector4(args.Transformation[0], args.Transformation[1], args.Transformation[2], args.Transformation[3]);
        Vector4 peakColorChangeDiff = new Vector4(args.PeakTransformationDiff[0], args.PeakTransformationDiff[1], args.PeakTransformationDiff[2], args.PeakTransformationDiff[3]);

        colorChange = (float)delta * (colorChange + (peakColorChangeDiff * magnitude));

        item.ChangeTintAdditive(colorChange);
    };

    public ITransformable AffectedItem { get { return _affectedItem; } }
    public string Tag { get { return _tag; } }
}


public class TransformaionRuleDelegateArgs
{
    public ReadOnlyCollection<float> Transformation { get; }
    public ReadOnlyCollection<float> PeakTransformationDiff { get; }
    public Vector2 Origin { get; }

    public TransformaionRuleDelegateArgs(IEnumerable<float> transformation, IEnumerable<float> peakTransformationDiff, Vector2 origin)
    {
        Transformation = transformation.ToList().AsReadOnly();
        PeakTransformationDiff = peakTransformationDiff.ToList().AsReadOnly();
        Origin = origin;
    }
}

