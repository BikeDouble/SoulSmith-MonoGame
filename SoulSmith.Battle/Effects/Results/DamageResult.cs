using SoulSmith.Battle.Effects.Damage;
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
        public DamageResult(IReadOnlyUnit sender, IReadOnlyUnit target, int effectiveDamage, DamageType damageType, Result parentResult)
            : base(sender, target, parentResult)
        {
            EffectiveDamage = effectiveDamage;
            DamageType = damageType;
        }

        public int EffectiveDamage { get; }
        public DamageType DamageType { get; }
    }
}
