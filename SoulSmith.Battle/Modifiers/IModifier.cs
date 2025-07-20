using SoulSmith.Battle.Effects;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle.Modifiers
{
    public interface IModifier : IReadOnlyModifier
    {
        public void ProcessEffectResult(EffectResult result);
        public void InterceptEffectRequest(EffectRequest request);
        public void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host);
        public StatModifier? GetStatModifier();
        public IReadOnlyUnit Applier { get; }
        public IReadOnlyUnit Host { get; }
        public bool IsVisible { get; }
        public DrawableResourceKey IconKey { get; }
        public ModifierAlignment Alignment { get; }
        public EventHandler<RemoveModifierEventArgs> RemoveModifierEventHandler { get; set; }
        public EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler { get; set; }
        public string Name { get; }
        public string Description { get; }
    }

    public class RemoveModifierEventArgs : EventArgs
    {
        public IModifier Modifier { get; set; }
    }
}
