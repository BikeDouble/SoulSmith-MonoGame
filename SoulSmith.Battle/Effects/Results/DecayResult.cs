using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class DecayResult : Result
    {
        public DecayResult(IReadOnlyUnit sender, IReadOnlyUnit target, int effectiveDecay, Result parentResult, Payload payload)
            : base(sender, target, parentResult, payload)
        {
            EffectiveDecay = effectiveDecay;
        }

        public int EffectiveDecay { get; }
    }
}
