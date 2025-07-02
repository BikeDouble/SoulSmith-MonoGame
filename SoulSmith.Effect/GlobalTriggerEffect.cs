using SoulSmith.Battle;
using SoulSmith.Battle.Effect;
using SoulSmith.Effect.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Effect
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
