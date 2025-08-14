using SoulSmith.Battle.Effects.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class TriggerPayload : Payload
    {
        public TriggerPayload(IReadOnlyUnit sender, IReadOnlyUnit target, CombatTrigger trigger, IReadOnlyEffect generatingEffect, IEffectOriginator originator) : base(sender, target, null, generatingEffect, originator, null)
        {
            Trigger = trigger;
        }

        public CombatTrigger Trigger { get; }
    }
}
