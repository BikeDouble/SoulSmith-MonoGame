using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Modifiers.Effect;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle.Modifiers.Payload
{
    public class StaticPayloadModifier : Modifier, IReadOnlyPayloadModifier
    {
        public EffectModifierTriggerStyle TriggerStyle { get; private set; }

        public StaticPayloadModifier(
            EffectModifierTriggerStyle triggerStyle,
            StatModStyle modStyle,
            float modAmount,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            IEffectOriginator originator,
            bool isVisible,
            string iconKey,
            string friendlyName,
            string description,
            string mergeKey)
            : base(duration, durationStyle, alignment, originator, isVisible, iconKey, friendlyName, description, null, mergeKey)
        {
            ModStyle = modStyle;
            ModAmount = modAmount;
            TriggerStyle = triggerStyle;
        }

        public override void ModifyPayload(PayloadBase payload)
        {
            base.ModifyPayload(payload);

            switch (TriggerStyle)
            {
                case EffectModifierTriggerStyle.OnGivingHitDamage:
                    if (payload.Sender == Host)
                    {
                        if (payload is DamagePayload damagePayload)
                        {
                            damagePayload.AddMagnitudeModifier(new MagnitudeModifier(ModStyle, ModAmount, this));
                        }
                        else if (payload is AOEDamagePayload aoeDamagePayload)
                        {
                            aoeDamagePayload.AddAllTargetMagnitudeModifier(new MagnitudeModifier(ModStyle, ModAmount, this));
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException($"TriggerStyle {TriggerStyle} is not valid for StaticDamagePayloadModifier.");
            }
        }

        private string GetStatusTextInternal()
        {
            string ret;

            switch (ModStyle)
            {
                case StatModStyle.AdditivePercent:
                case StatModStyle.MultiplicativePercent:
                    ret = ((int)ModAmount).ToString() + "%";
                    break;
                default:
                    ret = ((int)ModAmount).ToString();
                    break;
            }

            return ret;
        }

        protected override bool MergeInternal(IModifier other)
        {
            if (!(other is StaticPayloadModifier otherDamagePayloadModifier)) return false;
            if (otherDamagePayloadModifier.ModStyle != ModStyle) return false;

            ModAmount = StatTypeHelper.CombineModifiers(ModAmount, otherDamagePayloadModifier.ModAmount, ModStyle);

            return true;
        }

        public StatModStyle ModStyle { get; private set; }
        public override string StatusText { get { return GetStatusTextInternal(); } }
        public float ModAmount { get; private set; }
    }
}
