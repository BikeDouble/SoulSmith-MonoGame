using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class HealingResult : Result
    {
        public HealingResult(IReadOnlyUnit sender, IReadOnlyUnit target, int effectiveHealing, Result parentResult)
            : base(sender, target, parentResult)
        {
            EffectiveHealing = effectiveHealing;
        }

        public int EffectiveHealing { get; }
    }
}
