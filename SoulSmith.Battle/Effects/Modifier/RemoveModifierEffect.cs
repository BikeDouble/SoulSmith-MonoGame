using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Modifier
{
    public class RemoveModifierEffect : VisualizedEffectBase, IEffect
    {
        private IModifier _modifierToRemove;

        public RemoveModifierEffect(
            IModifier modifierToRemove,
            EffectVisualizationFactory visualizationFactory,
            float additionalDelay
        ) : base(TargetingStyle.Special, visualizationFactory, additionalDelay)
        {
            _modifierToRemove = modifierToRemove;
        }

        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null)
        {
            IModifier modifier = _modifierToRemove;

            return new RemoveModifierPayload(sender, modifier.Host, modifier, parentEffectResult, this, originator, ImmediateAfterEffects);
        }
    }
}
