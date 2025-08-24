using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization;
using SoulSmith.Battle.Effects.Payloads;

namespace SoulSmith.Battle.Effects.Trigger
{
    public class GlobalTriggerEffect : IEffect
    {
        private CombatTrigger _trigger;

        public GlobalTriggerEffect(CombatTrigger trigger) 
        {
            _trigger = trigger;
        }

        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null)
        {
            return new TriggerPayload(sender, target, _trigger, this, originator);
        }

        public EffectVisualization CreateVisualization()
        {
            return null;
        }

        public void Dispose() { }

        public float AdditionalDelay { get { return 0f; } }
        public bool HasVisualization { get { return false; } }
    }
}
