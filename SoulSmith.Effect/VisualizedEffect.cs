using SoulSmith.Effect.Visualization;

namespace SoulSmith.Effect
{
    public class VisualizedEffect
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
    }
}
