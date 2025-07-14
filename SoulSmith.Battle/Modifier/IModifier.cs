using SoulSmith.Battle.Effect;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle.Modifier
{
    public interface IModifier : IReadOnlyModifier
    {
        public void ProcessEffectResult(EffectResult result);
        public void InterceptEffectRequest(EffectRequest request);
        public void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host);
        public StatModifier? GetStatModifier();
        public IReadOnlyUnit Applier { get; }
        public IReadOnlyUnit Host { get; }
        public IReadOnlyCanvasObject Icon { get; }
        public bool IsVisible { get; }
    }

    public class RemoveModifierEventArgs : EventArgs
    {
        public IModifier Modifier { get; set; }
    }
}
