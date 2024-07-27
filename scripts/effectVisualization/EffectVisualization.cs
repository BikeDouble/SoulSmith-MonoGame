

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;

public partial class EffectVisualization : CanvasItem
{
    private double _lifespan = 3f; //Time in seconds before visualization automatically completes
    private bool _enabled = false;
    private double _delay = 0f;

    public event EventHandler<ReadyEffectEventArgs> ReadyEffectEventHandler;

    public EffectVisualization(EffectVisualization other) : base(other)
    {
        //TODO remove comment
        _lifespan = other._lifespan;
        _enabled = other._enabled;
        _delay = other._delay;
            
    }

    public EffectVisualization()
    {
        Hide();
        //Pause();
    }

    public override void Process(double delta)
    {
        if (_enabled)
        {
            OnProcess(delta);
        }
        else
        {
            _delay -= delta;
            if (_delay <= 0)
            {
                EnableVisualization();
            }
        }
        base.Process(delta);
    }

    //Tells the combat manager to apply the effect now, so that it is synced with the visualization
    public virtual void EmitReadyEffect()
    {
        ReadyEffectEventArgs e = new ReadyEffectEventArgs();

        ReadyEffectEventHandler(this, e);
    }

    public virtual void BeginVisualization(Unit user, Unit target, double delay = 0f)
    {
        _delay = delay;
        Hide();
        //Pause();
    }

    public virtual void EnableVisualization()
    {
        _enabled = true;
        Show();
        //Play();
    }

    public virtual void EndVisualization()
    {
        Hide();
    }

    public void LoadSprite(TrackedResource<CanvasItem_TransformationRules> sprite)
    {
        AddChild(sprite);
    }

    public virtual void OnProcess(double delta)
    {
        _lifespan -= delta;
        if (_lifespan <= 0)
        {
            EmitReadyEffect();
            EndVisualization();
        }
    }

    public override object DeepClone()
    {
        return new EffectVisualization(this);
    }

    public void SetLifespan(double lifespan) 
    {
        _lifespan = lifespan;
    }

}

public class ReadyEffectEventArgs : EventArgs
{

}