using SoulSmith.Battle.Effects.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class DecayPayload : PayloadBase
    {
        public DecayPayload(IReadOnlyUnit sender, IReadOnlyUnit target, int rawDecay, ResultBase parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, generatingEffect, originator, immediateAfterEffects)
        {
            RawDecay = rawDecay;
        }

        public int RawDecay { get; }
    }
}
