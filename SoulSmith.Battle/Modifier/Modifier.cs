using SoulSmith.Battle.Effect;
using SoulSmith.Drawing;
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
        public Modifier(ModifierAlignment alignment, bool isVisible = true, DrawableResourceKey iconKey = null)
        {
            Alignment = alignment;
            IsVisible = isVisible;
            IconKey = iconKey;
        }
        public virtual void ProcessEffectResult(EffectResult result) { }
        public virtual void InterceptEffectRequest(EffectRequest request) { }
        public virtual void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host) 
        {
            Applier = applier;
            Host = host;
        }
        public virtual StatModifier? GetStatModifier() 
        {
            return null;
        }
        public IReadOnlyUnit Applier { get; private set; }
        public IReadOnlyUnit Host { get; private set; }
        public bool IsVisible { get; private set; }
        public DrawableResourceKey IconKey { get; private set; }
        public ModifierAlignment Alignment { get; private set; }
        public EventHandler<RemoveModifierEventArgs> RemoveModifierEventHandler { get; set; }
        protected void Remove()
        {
            RemoveModifierEventArgs e = new RemoveModifierEventArgs();
            e.Modifier = this;

            RemoveModifierEventHandler?.Invoke(this, e);
        }
        public EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler { get; set; }
        protected void EnqueueEffectInput(EffectInput effectInput)
        {
            EnqueueEffectInputEventArgs e = new();
            e.EffectInput = effectInput;

            EnqueueEffectInputEventHandler(this, e);
        }
    }
}
