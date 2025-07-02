
using SoulSmith.Battle.Effect.Visualization;

namespace SoulSmith.Battle.Effect
{
    public class GlobalTriggerEffect : IEffect
    {
        private EffectTrigger _trigger;

        public GlobalTriggerEffect(EffectTrigger trigger) 
        {
            _trigger = trigger;
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            return new EffectRequest(sender, target, _trigger);
        }

        public EffectVisualization CloneVisualization()
        {
            return null;
        }

        public void Dispose() { }
    }
}
