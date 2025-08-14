using SoulSmith.Drawing;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Effects.Visualization;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Battle.Modifiers.Effect
{
    public class EffectOnTakingHitModifier : Modifier
    {
        private readonly IEffect _effect;

        public EffectOnTakingHitModifier(
            IEffect effect,
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
            _effect = effect ?? throw new ArgumentNullException(nameof(effect));
        }

        public override void ReactToPayloadResult(Result result)
        {
            base.ReactToPayloadResult(result);

            if (result is DamageResult damageResult)
            {
                if (damageResult.Target != this.Host) return;

                if (damageResult.DamageType != Effects.Damage.DamageType.Hit) return;

                if (damageResult.EffectiveDamage <= 0) return;

                EffectInput effectInput = new EffectInput(_effect, Host, result.Sender, Priority.ReactionToEnemy, this, result);

                EnqueueEffectInput(effectInput);
            }
        }
    }
}
