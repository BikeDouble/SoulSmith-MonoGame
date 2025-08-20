using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Effects.Payloads;

namespace SoulSmith.Battle.Effects.Damage
{
    public class FlatDecayDamageEffect : VisualizedEffectBase, IEffect
    {
        private int _decayDamage;

        public FlatDecayDamageEffect(int decayDamage, TargetingStyle targetingStyle = TargetingStyle.Target, EffectVisualizationFactory visualizationFactory = null, float additionalDelay = 0) : base(targetingStyle, visualizationFactory, additionalDelay)
        {
            _decayDamage = decayDamage;
        }

        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null)
        {
            int rawDecay = _decayDamage;

            PayloadBase request = new DecayPayload(sender, GetTrueTarget(sender, target), rawDecay, parentEffectResult, this, originator, ImmediateAfterEffects);
            return request;
        }
    }
}
