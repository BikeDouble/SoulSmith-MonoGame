using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class DamagePayload : Payload
    {
        public DamagePayload(IReadOnlyUnit sender, IReadOnlyUnit target, int rawDamage, DamageType damageType, Result parentResult, IReadOnlyEffect generatingEffect, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, generatingEffect, immediateAfterEffects)
        {
            RawDamage = rawDamage;
            DamageType = damageType;
        }

        public int RawDamage { get; }
        public DamageType DamageType { get; }
    }
}
