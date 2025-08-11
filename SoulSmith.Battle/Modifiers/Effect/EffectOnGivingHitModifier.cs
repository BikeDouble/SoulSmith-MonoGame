using SoulSmith.Drawing;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Effects.Visualization;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Battle.Modifiers.Effect
{
    public class EffectOnGivingHitModifier : Modifier
    {
        private readonly IEffect _effect;

        public EffectOnGivingHitModifier(
            IEffect effect,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            bool isVisible,
            string iconKey,
            string friendlyName,
            string description,
            string statusText)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description, statusText)
        {
            _effect = effect ?? throw new ArgumentNullException(nameof(effect));
        }

        public override void ReactToPayloadResult(Result result)
        {
            base.ReactToPayloadResult(result);

            if (result is DamageResult damageResult)
            {
                if (damageResult.Sender != this.Host) return;

                if (damageResult.Target == null) return;

                if (damageResult.DamageType != Effects.Damage.DamageType.Hit) return;

                if (damageResult.EffectiveDamage <= 0) return;

                EffectInput effectInput = new EffectInput(_effect, Host, result.Target, Priority.ReactionToSelf);

                EnqueueEffectInput(effectInput, result);
            }
        }
    }
}
