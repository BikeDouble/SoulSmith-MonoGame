using DynamicExpresso;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Effects.Payloads;

namespace SoulSmith.Battle.Effects.Damage
{
    public class NaturalDamageDecayEffect : VisualizedEffectBase, IEffect
    {
        private float _percentOfDamageAsDecay;

        public NaturalDamageDecayEffect(float percentOfDamageAsDecay, EffectVisualizationFactory visualizationFactory = null, float additionalDelay = 0) : base(visualizationFactory, additionalDelay)
        {
            _percentOfDamageAsDecay = percentOfDamageAsDecay;
        }

        public Payload GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, Result parentEffectResult = null)
        {
            int rawDecay = 0;

            if (parentEffectResult is DamageResult parentDamageResult) rawDecay = (int)(parentDamageResult.EffectiveDamage * _percentOfDamageAsDecay);

            Payload request = new DecayPayload(parentEffectResult?.Sender, parentEffectResult?.Target, rawDecay, parentEffectResult, this, ImmediateAfterEffects);

            return request;
        }
    }
}
