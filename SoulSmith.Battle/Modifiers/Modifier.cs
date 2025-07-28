using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Modifier;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers
{
    public class Modifier : IModifier
    {
        public Modifier(int duration, DurationStyle durationStyle, ModifierAlignment alignment, bool isVisible = true, string iconKey = null, string friendlyName = "Unnamed", string description = "")
        {
            Duration = duration;
            DurationStyle = durationStyle;
            Alignment = alignment;
            IsVisible = isVisible;
            IconKey = iconKey;
            Name = friendlyName;
            Description = description;
            RemovalEffect = new RemoveModifierEffect(this, null, 0);
        }
        public virtual void ReactToPayloadResult(Result result) 
        {
            if (result == null) return;
            
            switch(DurationStyle)
            {
                case DurationStyle.Rounds:
                    if (result is TriggerResult triggerResult1)
                    {
                        if (triggerResult1.Trigger == Effects.Trigger.CombatTrigger.OnRoundEnd)
                        {
                            DecrementDuration(result);
                        }
                    }
                    break;
                case DurationStyle.HostMoves:
                    if (result is TriggerResult triggerResult2)
                    {
                        if (triggerResult2.Trigger == Effects.Trigger.CombatTrigger.OnMoveEnd)
                        {
                            if (result.Sender == Host)
                            {
                                DecrementDuration(result);
                            }
                        }
                    }
                    break;
            }
        }

        private void DecrementDuration(Result parentResult)
        {
            Duration -= 1;

            if (Duration <= 0) EnqueueRemove(Priority.ModifierRemovalImmediate, parentResult);
        }

        public virtual void ModifyPayload(Payload request) { }
        public virtual void ApplyModifier(IReadOnlyUnit applier, IReadOnlyUnit host) 
        {
            Applier = applier;
            Host = host;
        }
        public virtual StatModifier? GetStatModifier() 
        {
            return null;
        }
        public int Duration { get; private set; }
        public DurationStyle DurationStyle { get; private set; }
        public IReadOnlyUnit Applier { get; private set; }
        public IReadOnlyUnit Host { get; private set; }
        public bool IsVisible { get; private set; }
        public string IconKey { get; private set; }
        public ModifierAlignment Alignment { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public RemoveModifierEffect RemovalEffect { get; private set; }
        protected void EnqueueRemove(Priority priority, Result parentResult)
        {
            EffectInput removeEffectInput = new EffectInput(RemovalEffect, Applier, Host, priority);
            EnqueueEffectInput(removeEffectInput);
        }
        public EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler { get; set; }
        protected void EnqueueEffectInput(EffectInput effectInput, Result parentResult = null)
        {
            EnqueueEffectInputEventArgs e = new();
            e.EffectInput = effectInput;
            e.ParentEffectResult = parentResult;

            EnqueueEffectInputEventHandler(this, e);
        }
    }
}
