using SoulSmith.Drawing;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Effects.Visualization;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Battle.Modifiers.Effect
{
    public class EffectOnResultReactionModifier : Modifier
    {
        private readonly IEffect _effect;
        private readonly EffectModifierTriggerStyle _trigger;
        private readonly Priority _effectPriority;
        private readonly string _desiredModifierMergeKey;

        public EffectOnResultReactionModifier(
            IEffect effect,
            Priority effectPriority,
            EffectModifierTriggerStyle trigger,
            string desiredModifierMergeKey,
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
            _trigger = trigger;
            _effectPriority = effectPriority;
            _desiredModifierMergeKey = desiredModifierMergeKey;
        }

        public override void ReactToPayloadResult(ResultBase result)
        {
            base.ReactToPayloadResult(result);

            switch (_trigger)
            {
                case EffectModifierTriggerStyle.OnGivingHitDamage:
                    if (result is DamageResult damageResult)
                    {
                        if (damageResult.Sender != this.Host) return;

                        if (damageResult.Target == null) return;

                        if (damageResult.DamageType != Effects.Damage.DamageType.Hit) return;

                        if (damageResult.EffectiveDamage <= 0) return;

                        TriggerEffect(damageResult.Target, result);
                    }
                    break;
                case EffectModifierTriggerStyle.OnTakingHitDamage:
                    if (result is DamageResult damageResult2)
                    {
                        if (damageResult2.Target != this.Host) return;

                        if (damageResult2.DamageType != Effects.Damage.DamageType.Hit) return;

                        if (damageResult2.EffectiveDamage <= 0) return;

                        EffectInput effectInput = new EffectInput(_effect, Host, result.Sender, Priority.Reaction, this, result);

                        TriggerEffect(damageResult2.Sender, result);
                    }
                    break;
                case EffectModifierTriggerStyle.OnHostRemovesOtherModifierFromSelf:
                    if (result is RemoveModifierResult removeModifierResult)
                    {
                        if (removeModifierResult.Modifier.MergeKey == _desiredModifierMergeKey)
                        {
                            if (removeModifierResult.Modifier.Host == this.Host)
                            {
                                if (removeModifierResult.Sender == this.Host)
                                {
                                    TriggerEffect(result.Target, result);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void TriggerEffect(IReadOnlyUnit target, ResultBase parentResult)
        {
            EffectInput effectInput = new EffectInput(_effect, Host, target, _effectPriority, this, parentResult);

            EnqueueEffectInput(effectInput);
        }
    }
}
