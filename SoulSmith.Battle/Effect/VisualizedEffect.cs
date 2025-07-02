using SoulSmith.Battle.Effect.Visualization;

namespace SoulSmith.Battle.Effect
{
    public class VisualizedEffect : IDisposable
    {
        private EffectVisualization _visualization;

        public VisualizedEffect(EffectVisualization visualization) 
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
