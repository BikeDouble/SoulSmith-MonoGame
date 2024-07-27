
using System;
using SoulSmithMoves;

public class EffectVisualizationListener
{
    // Proxy for EffectVisualization and EffectQueue
    private EffectVisualization _visualization;
    private bool _readyForExecute = true;

    public EffectVisualizationListener(EffectInput input)
    {
        EffectVisualization visualization = input.Effect.Visualization;
        if (visualization == null)
        {
            return;
        }
        BeginVisualization(input.Sender,
                           input.Target,
                           visualization,
                           input.Effect.VisualizationDelay);
    }

    public void BeginVisualization(Unit user,
                                   Unit target,
                                   EffectVisualization visualization,
                                   double delay = 0f)
    {
        _visualization = visualization;
        if (_visualization == null)
        {
            return;
        }
        _readyForExecute = false;
        _visualization.ReadyEffectEventHandler += OnVisualizationExecuteEffect;
        _visualization.BeginVisualization(user, target, delay);
    }

    private void OnVisualizationExecuteEffect(object sender, ReadyEffectEventArgs e)
    {
        _readyForExecute = true;
    }

    public EffectVisualization Visualization { get { return _visualization; } }
    public bool ReadyForExecute {  get { return _readyForExecute; } }
}
