using SoulSmith.Battle.Effect;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifier
{
    public class Modifier : IModifier
    {
        public virtual void ProcessEffectResult(EffectResult result) { }
        public virtual void InterceptEffectRequest(EffectRequest request) { }
        public virtual void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host) { }
        public virtual StatModifier? GetStatModifier() 
        {
            return null;
        }
        public IReadOnlyUnit Applier { get; }sdads TODO
        public IReadOnlyUnit Host { get; }
        public IReadOnlyCanvasObject Icon { get; }
        public bool IsVisible { get; }

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
    }
}
