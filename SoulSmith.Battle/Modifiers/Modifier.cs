using SoulSmith.Battle.Effects;
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
        public Modifier(int duration, DurationStyle durationStyle, ModifierAlignment alignment, bool isVisible = true, DrawableResourceKey iconKey = null, string friendlyName = "Unnamed", string description = "")
        {
            Duration = duration;
            DurationStyle = durationStyle;
            Alignment = alignment;
            IsVisible = isVisible;
            IconKey = iconKey;
            Name = friendlyName;
            Description = description;
        }
        public virtual void ReactToPayloadResult(Result result) 
        {
            if (result == null) return;
            
            switch(DurationStyle)
            {
                case DurationStyle.Rounds:
                    if (result is TriggerResult triggerResult1)
                    {
                        if (triggerResult1.Trigger == Effects.Trigger.CombatTrigger.OnRoundEndModifierDurationTick)
                        {
                            DecrementDuration();//TODO make sure modifiers removed after all other triggered effects
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
                                DecrementDuration();
                            }
                        }
                    }
                    break;
            }
        }

        private void DecrementDuration()
        {
            Duration -= 1;

            if (Duration <= 0) Remove();
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
        public DrawableResourceKey IconKey { get; private set; }
        public ModifierAlignment Alignment { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public EventHandler<RemoveModifierEventArgs> RemoveModifierEventHandler { get; set; }
        protected void Remove()
        {
            RemoveModifierEventArgs e = new RemoveModifierEventArgs();
            e.Modifier = this;

            RemoveModifierEventHandler?.Invoke(this, e);
        }
        public EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler { get; set; }
        protected void EnqueueEffectInput(EffectInput effectInput, Result parentEffectResult = null)
        {
            EnqueueEffectInputEventArgs e = new();
            e.EffectInput = effectInput;
            e.ParentEffectResult = parentEffectResult;

            EnqueueEffectInputEventHandler(this, e);
        }
    }
}
