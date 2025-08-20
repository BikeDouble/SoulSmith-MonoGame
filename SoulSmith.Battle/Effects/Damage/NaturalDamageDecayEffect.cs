using DynamicExpresso;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Effects.Payloads;

namespace SoulSmith.Battle.Effects.Damage
{
    public class NaturalDamageDecayEffect : VisualizedEffectBase, IEffect
    {
        private float _percentOfDamageAsDecay;

        public NaturalDamageDecayEffect(float percentOfDamageAsDecay, TargetingStyle targetingStyle = TargetingStyle.Special, EffectVisualizationFactory visualizationFactory = null, float additionalDelay = 0) : base(targetingStyle, visualizationFactory, additionalDelay)
        {
            _percentOfDamageAsDecay = percentOfDamageAsDecay;
        }

        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null)
        {
            int rawDecay = 0;

            if (parentEffectResult is DamageResult parentDamageResult) rawDecay = (int)(parentDamageResult.EffectiveDamage * _percentOfDamageAsDecay);

            PayloadBase request = new DecayPayload(parentEffectResult?.Sender, parentEffectResult?.Target, rawDecay, parentEffectResult, this, originator, ImmediateAfterEffects);

            return request;
        }
    }
}
