using SoulSmith.Battle.Effects.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class DecayPayload : Payload
    {
        public DecayPayload(IReadOnlyUnit sender, IReadOnlyUnit target, int rawDecay, Result parentResult, IReadOnlyEffect generatingEffect, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, generatingEffect, immediateAfterEffects)
        {
            RawDecay = rawDecay;
        }

        public int RawDecay { get; }
    }
}
