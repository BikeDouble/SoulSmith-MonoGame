using DynamicExpresso;
using SoulSmith.Battle.Effects.Visualization.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Damage
{
    public class NaturalDamageDecayEffect : VisualizedEffectBase, IEffect
    {
        private float _percentOfDamageAsDecay;

        public NaturalDamageDecayEffect(float percentOfDamageAsDecay, EffectVisualizationFactory visualizationFactory = null, float additionalDelay = 0) : base(visualizationFactory, additionalDelay)
        {
            _percentOfDamageAsDecay = percentOfDamageAsDecay;
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            int rawDecay = 0;

            if (parentEffectResult != null) rawDecay = (int)(parentEffectResult.EffectiveDamage * _percentOfDamageAsDecay);

            EffectRequest request = new EffectRequest(parentEffectResult?.Sender, parentEffectResult?.Target, DamageType.Decay, rawDecay);
            return request;
        }
    }
}
