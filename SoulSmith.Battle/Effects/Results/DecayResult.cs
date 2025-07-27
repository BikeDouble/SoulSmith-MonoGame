using SoulSmith.Battle.Effects.Damage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class DecayResult : Result
    {
        public DecayResult(IReadOnlyUnit sender, IReadOnlyUnit target, int effectiveDecay, Result parentResult)
            : base(sender, target, parentResult)
        {
            EffectiveDecay = effectiveDecay;
        }

        public int EffectiveDecay { get; }
    }
}
