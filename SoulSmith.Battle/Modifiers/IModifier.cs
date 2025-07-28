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
        public void ReactToPayloadResult(Result result);
        public void ModifyPayload(Payload request);
        public void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host);
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
        public string Name { get; }
        public string Description { get; }
    }

    public class RemoveModifierEventArgs : EventArgs
    {
        public IModifier Modifier { get; set; }
    }
}
