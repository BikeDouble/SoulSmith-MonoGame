
using System;
using System.Diagnostics;
using SoulSmithMoves;

public class EffectVisualizationListener
{
    // Proxy for EffectVisualization and EffectQueue
    private EffectVisualization _visualization;
    private bool _readyForExecute = true;

    public EffectVisualizationListener(EffectInput input, double additionalDelay)
    {
        EffectVisualization visualization = input.Effect.VisualizationTemplate?.Instantiate();
        if (visualization == null)
        {
            if (additionalDelay > 0)
            {
                StartTimerVisualization(additionalDelay);
            }
            return;
        }
        BeginVisualization(input.Sender,
                           input.Target,
                           visualization,
                           input.Effect.VisualizationDelay + additionalDelay);
    }

    public void BeginVisualization(IReadOnlyUnit user,
                                   IReadOnlyUnit target,
                                   EffectVisualization visualization,
                                   double delay = 0f)
    {
        _visualization = visualization;
        if (_visualization == null)
        {
            return;
        }
        _readyForExecute = false;
        _visualization.UpdateState(0);
        _visualization.ReadyEffectEventHandler += OnVisualizationExecuteEffect;
        _visualization.BeginVisualization(user, target, (float)delay);
    }

    private void StartTimerVisualization(double time)
    {
        EffectVisualization emptyVis = new EffectVisualization(null, null, null, (float)time);

        BeginVisualization(null, null, emptyVis, 0);
    }

    private void OnVisualizationExecuteEffect(object sender, ReadyEffectEventArgs e)
    {
        _readyForExecute = true;
        _visualization.ReadyEffectEventHandler -= OnVisualizationExecuteEffect;
    }

    public EffectVisualization Visualization { get { return _visualization; } }
    public bool ReadyForExecute {  get { return _readyForExecute; } }
}
