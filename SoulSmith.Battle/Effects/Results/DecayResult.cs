using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class DecayResult : ResultBase
    {
        public DecayResult(IReadOnlyUnit sender, IReadOnlyUnit target, int effectiveDecay, ResultBase parentResult, PayloadBase payload, IEffectOriginator originator)
            : base(sender, target, parentResult, payload, originator)
        {
            EffectiveDecay = effectiveDecay;
        }

        public int EffectiveDecay { get; }
    }
}
