using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Effects.Payloads;

namespace SoulSmith.Battle.Effects.Damage
{
    public class FlatDecayDamageEffect : VisualizedEffectBase, IEffect
    {
        private int _decayDamage;

        public FlatDecayDamageEffect(int decayDamage, EffectVisualizationFactory visualizationFactory = null, float additionalDelay = 0) : base(visualizationFactory, additionalDelay)
        {
            _decayDamage = decayDamage;
        }

        public Payload GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, Result parentEffectResult = null)
        {
            int rawDecay = _decayDamage;

            Payload request = new DecayPayload(sender, target, rawDecay, parentEffectResult);
            return request;
        }
    }
}
