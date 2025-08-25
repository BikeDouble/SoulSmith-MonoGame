using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Modifier;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle.Modifiers
{
    public interface IModifier : IReadOnlyModifier
    {
        public void ReactToPayloadResult(ResultBase result);
        public void ModifyPayload(PayloadBase request);
        public void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host);
        public bool TryMerge(IModifier other);
        public StatModifier? GetStatModifier();
        public IReadOnlyUnit Applier { get; }
        public IReadOnlyUnit Host { get; }
        public bool IsVisible { get; }
        public string IconKey { get; }
        public DurationStyle DurationStyle { get; }
        public int Duration { get; }
        public ModifierAlignment Alignment { get; }
        public EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler { get; set; }
        public RemoveModifierEffect RemovalEffect { get; }
        public string FriendlyName { get; }
        public string Description { get; }
        public string StatusText { get; }
        public string MergeKey { get; }
    }

    public class RemoveModifierEventArgs : EventArgs
    {
        public IReadOnlyModifier Modifier { get; set; }
    }
}
