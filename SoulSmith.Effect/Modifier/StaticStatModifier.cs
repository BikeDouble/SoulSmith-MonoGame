using SoulSmith.Battle;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;

namespace SoulSmith.Effect.Modifier
{
    public class StaticStatModifier : IModifier
    {
        public EventHandler<RemoveModifierEventArgs> RemoveModifierEventHandler { get; set; }
        public EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler { get; set; }

        public StaticStatModifier(StatType statType, int flatMod, double additiveMod, double multiplicativeMod, bool isVisible = true, IReadOnlyCanvasObject icon = null) 
        {
            StatType = statType;
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
            IsVisible = isVisible;
            Icon = icon;
        }

        public void ProcessEffectResult(EffectResult result)
        {
            return;
        }

        public void InterceptEffectRequest(EffectRequest request)
        {
            return;
        }

        public void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host)
        {
            Applier = applier;
            Host = host;

            return;
        }

        public StatModifier GetStatModifier()
        {
            return new StatModifier(StatType, FlatMod, AdditiveMod, MultiplicativeMod);
        }

        public StatType StatType { get; private set; }
        public int FlatMod { get; private set; }
        public double AdditiveMod { get; private set; }
        public double MultiplicativeMod { get; private set; }
        public IReadOnlyUnit Applier { get; private set; }
        public IReadOnlyUnit Host { get; private set; }
        public IReadOnlyCanvasObject Icon { get; private set; }
        public bool IsVisible { get; private set; }
    }
}
