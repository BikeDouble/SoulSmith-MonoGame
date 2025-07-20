using SoulSmith.Battle.Effects.Visualization;

namespace SoulSmith.Battle.Effects.Trigger
{
    public class GlobalTriggerEffect : IEffect
    {
        private CombatTrigger _trigger;

        public GlobalTriggerEffect(CombatTrigger trigger) 
        {
            _trigger = trigger;
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            return new EffectRequest(sender, target, _trigger);
        }

        public EffectVisualization CreateVisualization()
        {
            return null;
        }

        public void Dispose() { }
    }
}
