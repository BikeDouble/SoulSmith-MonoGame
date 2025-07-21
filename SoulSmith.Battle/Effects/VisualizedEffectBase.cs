using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Effects.Visualization;

namespace SoulSmith.Battle.Effects
{
    public class VisualizedEffectBase : IDisposable
    {
        private EffectVisualizationFactory _visualizationFactory;
        private float _additionalDelay;

        public VisualizedEffectBase(EffectVisualizationFactory visualizationFactory, float additionalDelay) 
        {
            _visualizationFactory = visualizationFactory;
            _additionalDelay = additionalDelay;
        }

        public EffectVisualization CreateVisualization()
        {
            return _visualizationFactory?.CreateVisualization();
        }

        public virtual void Dispose()
        {
            _visualizationFactory?.Dispose();
        }

        public float AdditionalDelay { get { return _additionalDelay; } }
    }
}
