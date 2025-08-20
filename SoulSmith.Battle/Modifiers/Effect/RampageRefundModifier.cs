using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Modifier;
using SoulSmith.Battle.Effects.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers.Effect
{
    public class RampageRefundModifier : Modifier
    {
        private readonly string _modifierToRefundMergeKey;
        private IReadOnlyModifier _modifierToRefund;
        private readonly IEffect _effect;

        public RampageRefundModifier(
            IEffect effect,
            string modifierToRefundMergeKey,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            IEffectOriginator originator,
            bool isVisible,
            string iconKey,
            string friendlyName,
            string description,
            string statusText)
            : base(duration, durationStyle, alignment, originator, isVisible, iconKey, friendlyName, description, statusText)
        {
            _modifierToRefundMergeKey = modifierToRefundMergeKey;
            _effect = effect ?? throw new ArgumentNullException(nameof(effect));
        }

        public override void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host)
        {
            base.ApplyModifier(applier, host);

            _modifierToRefund = host.ReadOnlyStats.GetReadOnlyModifier(_modifierToRefundMergeKey);
        }


        public override void ReactToPayloadResult(ResultBase result)
        {
            base.ReactToPayloadResult(result);

            if (result is RemoveModifierResult removeModifierResult && removeModifierResult.Modifier == _modifierToRefund) // Check if the modifier is the one to be refunded
            {
                if (result.ParentResult.ParentResult is AddModifierResult thisAddModifierResult && thisAddModifierResult.Modifier == this) // Check if the result that caused the modifier to be removed is the child of this modifiers add result
                {
                    if (result.ParentResult is DamageResult hitResult && hitResult.KilledTarget) // Check if the target of the damage was killed
                    {
                        // Refund the modifier
                        EffectInput effectInput = new EffectInput(_effect, Host, Host, Priority.Body, this, result);

                        EnqueueEffectInput(effectInput);

                        EnqueueRemove(Priority.ModifierRemovalImmediate, result);
                    }
                }
            }
        }
    }
}
