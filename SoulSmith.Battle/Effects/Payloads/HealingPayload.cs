using SoulSmith.Battle.Effects.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class HealingPayload : Payload
    {
        public HealingPayload(IReadOnlyUnit sender, IReadOnlyUnit target, int rawHealing, Result parentResult, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, immediateAfterEffects)
        {
            RawHealing = rawHealing;
        }

        public int RawHealing { get; }
    }
}
