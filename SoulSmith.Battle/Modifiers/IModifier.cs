using SoulSmith.Battle.Effects;
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
