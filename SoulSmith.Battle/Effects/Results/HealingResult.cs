using SoulSmith.Battle.Effects.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class HealingResult : ResultBase
    {
        public HealingResult(IReadOnlyUnit sender, IReadOnlyUnit target, int effectiveHealing, ResultBase parentResult, PayloadBase payload, IEffectOriginator originator)
            : base(sender, target, parentResult, payload, originator)
        {
            EffectiveHealing = effectiveHealing;
        }

        public int EffectiveHealing { get; }
    }
}
