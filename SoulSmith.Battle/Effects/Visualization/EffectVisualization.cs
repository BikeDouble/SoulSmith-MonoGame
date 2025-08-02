using SoulSmith.Object.Canvas;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SoulSmith.Battle.Effects.Visualization;
public class EffectVisualization : CanvasObject
{
    public const string FIREZONEZONEKEY = "firezone";
    public const string HITZONEZONEKEY = "hitzone";

    private float _totalLifespan = 3f; //Time in seconds before visualization automatically completes
    private float _elapsedLifespan = 0f;
    private float _effectActivationTimer = -1;
    private bool _enabled = false;
    private float _delay = 0f;
    private float _baseDelay = 0f;
    private IReadOnlyUnit _sender = null;
    private IReadOnlyUnit _target = null;

    public event EventHandler<ReadyEffectEventArgs> ReadyEffectEventHandler;

    public EffectVisualization(
        float lifespan,
        float effectActivationTimer = -1,
        float delay = 0f) : base()
    {
        _totalLifespan = lifespan;
        _elapsedLifespan = lifespan;
        _effectActivationTimer = effectActivationTimer;
        _delay = delay;
        _baseDelay = delay;
    }

    public override void Process(double delta)
    {
        if (_enabled)
        {
            EnabledProcess(delta);
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

    public virtual void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float additionalDelay = 0f)
    {
        _sender = sender;
        _target = target;

        _elapsedLifespan = 0;

        _delay += additionalDelay;

        if (_delay > 0)
            Hide();
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

    /// <summary>
    /// For processes to be done only if the EffectVisualization is active (not delayed or finished)
    /// </summary>
    /// <param name="delta"></param>
    protected virtual void EnabledProcess(double delta)
    {
        _elapsedLifespan += (float)delta;

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

    protected float TotalLifespan { get { return _totalLifespan; } }
    protected float ElapsedLifespan { get { return _elapsedLifespan; } }
    protected float Delay { get { return _delay; } }
    protected float BaseDelay { get { return _baseDelay; } }
    protected bool Enabled { get { return _enabled; } }
    protected float EffectActivationTimer { get { return _effectActivationTimer; } }
    protected IReadOnlyUnit Sender { get { return _sender; } }
    protected IReadOnlyUnit Target { get { return _target; } }
}

public class ReadyEffectEventArgs : EventArgs
{

}