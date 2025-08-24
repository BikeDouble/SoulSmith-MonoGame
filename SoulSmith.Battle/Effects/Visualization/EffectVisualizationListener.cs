namespace SoulSmith.Battle.Effects.Visualization;
public class EffectVisualizationListener
{
    // Proxy for EffectVisualization and EffectQueue
    private EffectVisualization _visualization;
    private bool _readyForExecute = true;

    public EffectVisualizationListener(EffectInput input, double delay, bool mirrorVisuals)
    {
        delay += input.Effect.AdditionalDelay;
        EffectVisualization visualization = input.Effect.CreateVisualization();
        if (visualization == null)
        {
            if (delay > 0)
            {
                StartTimerVisualization(delay);
            }
            return;
        }
        BeginVisualization(input.Sender,
                           input.Target,
                           visualization,
                           delay,
                           mirrorVisuals);
    }

    public void BeginVisualization(IReadOnlyUnit sender,
                                   IReadOnlyUnit target,
                                   EffectVisualization visualization,
                                   double delay = 0f,
                                   bool mirrorVisuals = false)
    {
        _visualization = visualization;
        if (_visualization == null)
        {
            return;
        }
        _readyForExecute = false;
        //_visualization.UpdateState(0); TODO
        _visualization.ReadyEffectEventHandler += OnVisualizationExecuteEffect;

        _visualization.BeginVisualization(sender, target, (float)delay, mirrorVisuals);
    }

    private void StartTimerVisualization(double time)
    {
        EffectVisualization emptyVis = new EffectVisualization((float)time, 0, (float)time);

        BeginVisualization(null, null, emptyVis, 0);
    }

    private void OnVisualizationExecuteEffect(object sender, ReadyEffectEventArgs e)
    {
        _readyForExecute = true;
        _visualization.ReadyEffectEventHandler -= OnVisualizationExecuteEffect;
    }

    public EffectVisualization Visualization { get { return _visualization; } }
    public bool ReadyForExecute { get { return _readyForExecute; } }
}
