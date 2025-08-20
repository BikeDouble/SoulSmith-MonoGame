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
        private readonly bool _enqueueEffectForAllTargetsOfAOE = true;

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
                        CheckAndEnqueueOnGivingHitDamageResult(damageResult);
                    }
                    else if (result is AOEDamageResult aoeDamageResult)
                    {
                        DamageResult primaryDamageResult = aoeDamageResult.GetResultOfPrimaryTarget();
                        if (primaryDamageResult != null) CheckAndEnqueueOnGivingHitDamageResult(primaryDamageResult);

                        if (_enqueueEffectForAllTargetsOfAOE)
                        {
                            foreach (IReadOnlyUnit target in aoeDamageResult.GetSecondaryTargets())
                            {
                                DamageResult damageResultOfTarget = aoeDamageResult.GetResultOfTarget(target);
                                if (damageResultOfTarget != null && damageResultOfTarget != primaryDamageResult)
                                {
                                    CheckAndEnqueueOnGivingHitDamageResult(damageResultOfTarget);
                                }
                            }
                        }
                    }
                    break;
                case EffectModifierTriggerStyle.OnTakingHitDamage:
                    if (result is DamageResult damageResult2)
                    {
                        CheckAndEnqueueOnTakingHitDamageResult(damageResult2);
                    }
                    else if (result is AOEDamageResult aoeDamageResult)
                    {
                        damageResult2 = aoeDamageResult.GetResultOfTarget(Host);

                        if (damageResult2 != null) CheckAndEnqueueOnTakingHitDamageResult(damageResult2);
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

        private void CheckAndEnqueueOnTakingHitDamageResult(DamageResult damageResult)
        {
            if (damageResult.Target != this.Host) return;

            if (damageResult.DamageType != Effects.Damage.DamageType.Hit) return;

            if (damageResult.EffectiveDamage <= 0) return;

            EffectInput effectInput = new EffectInput(_effect, Host, damageResult.Sender, Priority.Reaction, this, damageResult);

            TriggerEffect(damageResult.Sender, damageResult);
        }

        private void CheckAndEnqueueOnGivingHitDamageResult(DamageResult damageResult)
        {
            if (damageResult.Sender != this.Host) return;

            if (damageResult.Target == null) return;

            if (damageResult.DamageType != Effects.Damage.DamageType.Hit) return;

            if (damageResult.EffectiveDamage <= 0) return;

            TriggerEffect(damageResult.Target, damageResult);
        }

        private void TriggerEffect(IReadOnlyUnit target, ResultBase parentResult)
        {
            EffectInput effectInput = new EffectInput(_effect, Host, target, _effectPriority, this, parentResult);

            EnqueueEffectInput(effectInput);
        }
    }
}
