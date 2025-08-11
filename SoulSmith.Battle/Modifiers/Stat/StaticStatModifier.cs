using SoulSmith.Battle.Effects;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle.Modifiers.Stat
{
    public class StaticStatModifier : Modifier
    {
        public StaticStatModifier(
            StatType statType,
            StatModStyle modType,
            double modAmount,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            bool isVisible,
            string iconKey,
            string friendlyName,
            string description,
            string mergeKey)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description, null, mergeKey)
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
                    ret = ((int)(ModAmount * 100)).ToString() + "%";
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

            switch (ModStyle) {
                case StatModStyle.Flat:
                    ModAmount += otherStatModifier.ModAmount;
                    return true;
                case StatModStyle.AdditivePercent:
                    ModAmount += otherStatModifier.ModAmount;
                    return true;
                case StatModStyle.MultiplicativePercent:
                    ModAmount = (1 + 0.01 * ModAmount) * (1 + 0.01 * otherStatModifier.ModAmount) - 1;
                    return true;
                default:
                    return false;
            }
        }

        public StatType StatType { get; private set; }
        public StatModStyle ModStyle { get; private set; }
        public override string StatusText { get { return GetStatusTextInternal(); } }
        public double ModAmount { get; private set; }
    }
}
