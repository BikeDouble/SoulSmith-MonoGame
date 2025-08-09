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
            string description)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description)
        {
            StatType = statType;
            ModType = modType;
            ModAmount = modAmount;
        }


        public override StatModifier? GetStatModifier()
        {
            return new StatModifier(StatType, ModType, ModAmount);
        }

        public StatType StatType { get; private set; }
        public StatModStyle ModType { get; private set; }
        public double ModAmount { get; private set; }
    }
}
