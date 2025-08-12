using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class DamageResult : Result
    {
        public DamageResult(IReadOnlyUnit sender, IReadOnlyUnit target, int effectiveDamage, DamageType damageType, bool killedTarget, Result parentResult, Payload payload)
            : base(sender, target, parentResult, payload)
        {
            EffectiveDamage = effectiveDamage;
            DamageType = damageType;
            KilledTarget = killedTarget;
        }

        public int EffectiveDamage { get; }
        public DamageType DamageType { get; }
        public bool KilledTarget { get; }
    }
}
