using SoulSmith.Battle.Effects.Results;
using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class HealingPayload : MagnitudeModifiablePayloadBase, IMagnitudeModifiablePayload
    {
        public HealingPayload(IReadOnlyUnit sender, IReadOnlyUnit target, int rawHealing, ResultBase parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, generatingEffect, originator, immediateAfterEffects)
        {
            RawHealing = rawHealing;
        }

        public int RawHealing { get; }
        public int ModifiedAmount { get { return (int)StatTypeHelper.CombineAndApplyStyledModifiers(RawHealing, MagnitudeModifiers); } }
    }
}
