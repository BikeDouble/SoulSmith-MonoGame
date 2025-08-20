using SoulSmith.Battle.Effects;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle.Modifiers.Stat
{
    public class StaticStatModifier : Modifier, IReadOnlyStatModifier
    {
        public StaticStatModifier(
            StatType statType,
            StatModStyle modType,
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
            StatType = statType;
            ModStyle = modType;
            ModAmount = modAmount;
        }


        public override StatModifier? GetStatModifier()
        {
            return new StatModifier(StatType, ModStyle, ModAmount);
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
            if (!(other is StaticStatModifier otherStatModifier)) return false;
            if (otherStatModifier.StatType != StatType) return false;
            if (otherStatModifier.ModStyle != ModStyle) return false;

            ModAmount = StatTypeHelper.CombineModifiers(ModAmount, otherStatModifier.ModAmount, ModStyle);

            return true;
        }

        public StatType StatType { get; private set; }
        public StatModStyle ModStyle { get; private set; }
        public override string StatusText { get { return GetStatusTextInternal(); } }
        public float ModAmount { get; private set; }
    }
}
