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
        public StatModifier GetStatModifier();
        public EventHandler<RemoveModifierEventArgs> RemoveModifierEventHandler { get; set; }
        public event EventHandler<RemoveModifierEventArgs> RemoveModifierEvent
        {
            add => RemoveModifierEventHandler += value;
            remove => RemoveModifierEventHandler -= value;
        }
        protected void Remove()
        {
            RemoveModifierEventArgs e = new RemoveModifierEventArgs();
            e.Modifier = this;

            RemoveModifierEventHandler?.Invoke(this, e);
        }
        public EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler { get; set; }
        public event EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEvent
        {
            add => EnqueueEffectInputEventHandler += value;
            remove => EnqueueEffectInputEventHandler -= value;
        }
        protected void EnqueueEffectInput(EffectInput effectInput)
        {
            EnqueueEffectInputEventArgs e = new();
            e.EffectInput = effectInput;

            EnqueueEffectInputEventHandler(this, e);
        }
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
