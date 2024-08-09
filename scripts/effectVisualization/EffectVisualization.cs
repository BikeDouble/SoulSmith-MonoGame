

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;
using Microsoft.Xna.Framework;

public partial class EffectVisualization : CanvasItem_TransformationRules
{
    private float _totalLifespan = 3f; //Time in seconds before visualization automatically completes
    private float _remainingLifespan = 3f;
    private bool _enabled = false;
    private float _delay = 0f;
    private Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> _begin;
    private Func<EffectVisualizationProcessArgs, EffectVisualizationProcessOutput> _process;
    private Vector2 _startPoint = Vector2.Zero;
    private Vector2 _endPoint = Vector2.Zero;
    private List<float> _processParams = null;

    public event EventHandler<ReadyEffectEventArgs> ReadyEffectEventHandler;

    public EffectVisualization(EffectVisualization other) : base(other)
    {
        _totalLifespan = other._totalLifespan;
        _remainingLifespan = _totalLifespan;
        _enabled = other._enabled;
        _delay = other._delay;
        _begin = other._begin;
        _process = other._process;
    }

    public EffectVisualization(
        CanvasItem sprite,
        Func<EffectVisualizationBeginArgs, EffectVisualizationBeginOutput> begin,
        Func<EffectVisualizationProcessArgs, EffectVisualizationProcessOutput> process,
        float lifespan) : base()
    {
        AddChild(sprite);
        _begin = begin;
        _process = process;
        _totalLifespan = lifespan;
        _remainingLifespan = lifespan;
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

        EffectVisualizationBeginOutput output = _begin?.Invoke(args);

        ProcessBeginOutput(output);

        _remainingLifespan = _totalLifespan;

        Position.Set(_startPoint);

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

    private EffectVisualizationProcessArgs CreateProcessArgs()
    {
        EffectVisualizationProcessArgs args = new EffectVisualizationProcessArgs();
        args.Params = _processParams;
        args.TotalLifeSpan = _totalLifespan;
        args.ElapsedLifeSpan = _remainingLifespan;
        args.StartingPoint = _startPoint;
        args.EndingPoint = _endPoint;
        args.CurrentPosition = Position;
        
        return args;
    }

    private void ProcessProcessOutput(EffectVisualizationProcessOutput output)
    {
        if (output == null) return;

        Transform(output.Transformation);
    }

    public void EnableVisualization()
    {
        _enabled = true;
        Show();
        //Play();
    }

    public void EndVisualization()
    {
        Hide();
    }

    private void OnProcess(double delta)
    {
        _remainingLifespan -= (float)delta;

        ProcessProcessOutput(_process?.Invoke(CreateProcessArgs()));

        if (_remainingLifespan <= 0)
        {
            EmitReadyEffect();
            EndVisualization();
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