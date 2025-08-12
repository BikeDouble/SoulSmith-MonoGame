using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Effects.Visualization;

namespace SoulSmith.Battle.Effects
{
    public class VisualizedEffectBase : IDisposable
    {
        private EffectVisualizationFactory _visualizationFactory;
        private float _additionalDelay;

        public VisualizedEffectBase(EffectVisualizationFactory visualizationFactory, float additionalDelay, IEnumerable<IEffect> immediateAfterEffects = null) 
        {
            _visualizationFactory = visualizationFactory;
            _additionalDelay = additionalDelay;
            ImmediateAfterEffects = immediateAfterEffects;
        }

        public EffectVisualization CreateVisualization()
        {
            return _visualizationFactory?.CreateVisualization();
        }

        public virtual void Dispose()
        {
            _visualizationFactory?.Dispose();
        }

        protected IEnumerable<IEffect> ImmediateAfterEffects { get; private set; }
        public float AdditionalDelay { get { return _additionalDelay; } }
    }
}
