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
            int flatMod,
            double additiveMod,
            double multiplicativeMod,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            bool isVisible,
            DrawableResourceKey iconKey,
            string friendlyName,
            string description)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description)
        {
            StatType = statType;
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
        }


        public override StatModifier? GetStatModifier()
        {
            return new StatModifier(StatType, FlatMod, AdditiveMod, MultiplicativeMod);
        }

        public StatType StatType { get; private set; }
        public int FlatMod { get; private set; }
        public double AdditiveMod { get; private set; }
        public double MultiplicativeMod { get; private set; }
    }
}
