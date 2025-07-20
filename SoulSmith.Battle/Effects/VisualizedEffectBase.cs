using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Effects.Visualization;

namespace SoulSmith.Battle.Effects
{
    public class VisualizedEffectBase : IDisposable
    {
        private EffectVisualizationFactory _visualizationFactory;

        public VisualizedEffectBase(EffectVisualizationFactory visualizationFactory) 
        {
            _visualizationFactory = visualizationFactory;
        }

        public EffectVisualization CreateVisualization()
        {
            return _visualizationFactory?.CreateVisualization();
        }

        public virtual void Dispose()
        {
            _visualizationFactory?.Dispose();
        }
    }
}
