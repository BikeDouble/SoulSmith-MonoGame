using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.UnitStats;
using System.Collections.ObjectModel;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class DamagePayload : MagnitudeModifiablePayloadBase, IMagnitudeModifiablePayload
    {
        public DamagePayload(IReadOnlyUnit sender, IReadOnlyUnit target, int rawDamage, DamageType damageType, ResultBase parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, generatingEffect, originator, immediateAfterEffects)
        {
            RawDamage = rawDamage;
            DamageType = damageType;
        }

        public int RawDamage { get; }
        public int ModifiedAmount { get { return (int)StatTypeHelper.CombineAndApplyStyledModifiers(RawDamage, MagnitudeModifiers); } }
        public DamageType DamageType { get; }
    }
}
