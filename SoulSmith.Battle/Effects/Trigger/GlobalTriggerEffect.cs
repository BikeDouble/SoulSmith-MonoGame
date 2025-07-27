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

        public Payload GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, Result parentEffectResult = null)
        {
            return new TriggerPayload(sender, target, _trigger);
        }

        public EffectVisualization CreateVisualization()
        {
            return null;
        }

        public void Dispose() { }

        public float AdditionalDelay { get { return 0f; } }
    }
}
