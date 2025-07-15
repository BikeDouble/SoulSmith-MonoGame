using SoulSmith.Battle.Effect;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle.Modifier
{
    public class StaticRoundDurationStatModifier : RoundDurationModifier
    {
        public StaticRoundDurationStatModifier(StatType statType, int flatMod, double additiveMod, double multiplicativeMod, int duration, ModifierAlignment alignment = ModifierAlignment.Null, bool isVisible = true, DrawableResourceKey iconKey = null) 
            : base(duration, alignment, isVisible, iconKey)
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
