using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Effects.Visualization;

namespace SoulSmith.Battle.Effects
{
    public class VisualizedEffectBase : IDisposable
    {
        private EffectVisualizationFactory _visualizationFactory;
        private float _additionalDelay;

        public VisualizedEffectBase(TargetingStyle targetingStyle, EffectVisualizationFactory visualizationFactory, float additionalDelay, IEnumerable<IEffect> immediateAfterEffects = null)
        {
            _visualizationFactory = visualizationFactory;
            _additionalDelay = additionalDelay;
            ImmediateAfterEffects = immediateAfterEffects;
            TargetingStyle = targetingStyle;
        }

        public IReadOnlyUnit GetTrueTarget(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat)
        {
            return TargetingStyle switch
            {
                TargetingStyle.Sender => sender,
                TargetingStyle.Target => target,
                TargetingStyle.AcrossFromSender => combat.GetReadOnlyUnitAcrossFrom(sender),
                TargetingStyle.AnyEnemy => combat.GetAnyEnemyReadOnlyUnit(sender),
                _ => throw new InvalidOperationException($"Unsupported targeting style: {TargetingStyle}")
            };
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
        public TargetingStyle TargetingStyle { get; private set; }
        public bool HasVisualization { get { return _visualizationFactory != null; } }
    }
}
