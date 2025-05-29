

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Object;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Battle.Effect.Visualization;
public class EffectVisualization : CanvasObject
{
    private float _totalLifespan = 3f; //Time in seconds before visualization automatically completes
    private float _elapsedLifespan = 0f;
    private float _effectActivationTimer = -1;
    private bool _enabled = false;
    private float _delay = 0f;
    private Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> _begin;
    private Action<EffectVisualizationProcessArgs> _process;
    private Vector2 _startPoint = Vector2.Zero;
    private Vector2 _endPoint = Vector2.Zero;
    private List<float> _processParams = null;
    private ReadOnlyCollection<ITransformable> _transformables = null;

    public event EventHandler<ReadyEffectEventArgs> ReadyEffectEventHandler;

    public EffectVisualization(EffectVisualization other) : base(other)
    {
        _totalLifespan = other._totalLifespan;
        _elapsedLifespan = _totalLifespan;
        _enabled = other._enabled;
        _delay = other._delay;
        _begin = other._begin;
        _process = other._process;
        _transformables = CloneTransformables(
            other._transformables,
            Children,
            other.Children);
    }

    private static ReadOnlyCollection<ITransformable> CloneTransformables(
        ReadOnlyCollection<ITransformable> otherTransformables,
        ReadOnlyCollection<SoulSmithObject> children,
        ReadOnlyCollection<SoulSmithObject> otherChildren)
    {
        if (otherTransformables == null) return null;

        List<ITransformable> transformables = new();

        foreach (ITransformable item in otherTransformables)
        {
            int itemIndex = otherChildren.IndexOf(item as SoulSmithObject);

            CanvasObject transformable = children[itemIndex] as CanvasObject;

            if (transformable != null)
                transformables.Add(transformable);
        }

        if (transformables.Count < 1) return null;

        return transformables.AsReadOnly();
    }

    public EffectVisualization(
        CanvasObject sprite,
        Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> begin,
        Action<EffectVisualizationProcessArgs> process,
        float lifespan,
        float effectActivationTimer = -1) : base()
    {
        List<ITransformable> transformables = new List<ITransformable> { sprite };
        _transformables = transformables.AsReadOnly();
        AddChild(sprite);
        _begin = begin;
        _process = process;
        _totalLifespan = lifespan;
        _elapsedLifespan = lifespan;
        _effectActivationTimer = effectActivationTimer;
    }

    public override void Process(double delta)
    {
        if (_enabled)
        {
            OnProcess(delta);
        }
        else
        {
            _delay -= (float)delta;
            if (_delay <= 0)
            {
                EnableVisualization();
            }
        }
        base.Process(delta);
    }

    //Tells the combat manager to apply the effect now, so that it is synced with the visualization
    public void EmitReadyEffect()
    {
        ReadyEffectEventArgs e = new ReadyEffectEventArgs();

        ReadyEffectEventHandler?.Invoke(this, e);
    }

    public void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0f)
    {
        EffectVisualizationBeginArgs args = new();
        args.Sender = sender;
        args.Target = target;
        args.Transformables = _transformables;

        EffectVisualizationBeginOutput output = _begin?.Invoke(args);

        ProcessBeginOutput(output);

        _elapsedLifespan = 0;

        _delay = delay;
        
        if (_delay > 0)
            Hide();
    }

    private void ProcessBeginOutput(EffectVisualizationBeginOutput output)
    {
        if (output == null) return;

        _startPoint = output.StartingPoint; 
        _endPoint = output.EndingPoint;
        _processParams = output.Params;
    }

    private EffectVisualizationProcessArgs CreateProcessArgs(double delta)
    {
        EffectVisualizationProcessArgs args = new EffectVisualizationProcessArgs();
        args.Params = _processParams;
        args.TotalLifeSpan = _totalLifespan;
        args.ElapsedLifeSpan = _elapsedLifespan;
        args.StartingPoint = _startPoint;
        args.EndingPoint = _endPoint;
        args.Transformables = _transformables;
        args.Delta = delta;
        
        return args;
    }

    public void EnableVisualization()
    {
        _enabled = true;
        Show();
    }

    public event EventHandler EndVisualizationEventHandler;

    public void EndVisualization()
    {
        Hide();
        EndVisualizationEventHandler?.Invoke(this, EventArgs.Empty);
    }

    private void OnProcess(double delta)
    {
        _elapsedLifespan += (float)delta;

        _process?.Invoke(CreateProcessArgs(delta));

        if (_elapsedLifespan >= _totalLifespan)
        {
            _elapsedLifespan = _totalLifespan;
            EmitReadyEffect();
            EndVisualization();
        }

        if (_effectActivationTimer >= 0)
        {
            _effectActivationTimer -= (float)delta;

            if (_effectActivationTimer <= 0)
            {
                EmitReadyEffect();
                _effectActivationTimer = -1;
            }
        }
    }

    public override object DeepClone()
    {
        return new EffectVisualization(this);
    }
}

public class ReadyEffectEventArgs : EventArgs
{

}