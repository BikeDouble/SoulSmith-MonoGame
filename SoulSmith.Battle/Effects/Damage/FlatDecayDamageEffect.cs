using SoulSmith.Battle.Effects.Visualization.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Damage
{
    public class FlatDecayDamageEffect : VisualizedEffectBase, IEffect
    {
        private int _decayDamage;

        public FlatDecayDamageEffect(int decayDamage, EffectVisualizationFactory visualizationFactory = null, float additionalDelay = 0) : base(visualizationFactory, additionalDelay)
        {
            _decayDamage = decayDamage;
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            int rawDecay = _decayDamage;

            EffectRequest request = new EffectRequest(sender, target, DamageType.Decay, rawDecay);
            return request;
        }
    }
}
