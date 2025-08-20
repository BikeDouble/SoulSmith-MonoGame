using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class TriggerResult : ResultBase
    {
        public TriggerResult(IReadOnlyUnit sender, IReadOnlyUnit target, CombatTrigger trigger, PayloadBase payload, IEffectOriginator originator)
            : base(sender, target, null, payload, originator)
        {
            Trigger = trigger;
        }

        public CombatTrigger Trigger { get; }
    }
}
