using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class AOEDamageResult : AOEResultBase<DamageResult>
    {
        public AOEDamageResult(IReadOnlyUnit sender, IReadOnlyUnit primaryTarget, IReadOnlyCollection<DamageResult> individualResults, float fractionOfDamageToSecondaryTargets, ResultBase parentResult, PayloadBase payload, IEffectOriginator originator)
            : base(sender, primaryTarget, individualResults, parentResult, payload, originator)
        {
            FractionOfDamageToSecondaryTargets = fractionOfDamageToSecondaryTargets;
        }

        public float FractionOfDamageToSecondaryTargets { get; }
    }
}
