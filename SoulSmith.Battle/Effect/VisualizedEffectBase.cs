using SoulSmith.Battle.Effect.Visualization;

namespace SoulSmith.Battle.Effect
{
    public class VisualizedEffectBase : IDisposable
    {
        private EffectVisualization _visualization;

        public VisualizedEffectBase(EffectVisualization visualization) 
        {
            _visualization = visualization;
        }

        public EffectVisualization CloneVisualization()
        {
            return _visualization.CloneVisualization();
        }

        public void Dispose()
        {
            _visualization?.Dispose();
        }
    }
}
