using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.UnitStats;
using System.Collections.ObjectModel;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class DamagePayload : Payload
    {
        private readonly List<MagnitudeModifier> _magnitudeModifiers = new List<MagnitudeModifier>();

        public DamagePayload(IReadOnlyUnit sender, IReadOnlyUnit target, int rawDamage, DamageType damageType, Result parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, generatingEffect, originator, immediateAfterEffects)
        {
            RawDamage = rawDamage;
            DamageType = damageType;
        }

        public void AddMagnitudeModifier(MagnitudeModifier magnitudeModifier)
        {
            _magnitudeModifiers.Add(magnitudeModifier);
        }

        public int RawDamage { get; }
        public int ModifiedDamage { get { return (int)StatTypeHelper.CombineAndApplyStyledModifiers(RawDamage, _magnitudeModifiers); } }
        public DamageType DamageType { get; }
        public ReadOnlyCollection<MagnitudeModifier> MagnitudeModifiers { get { return _magnitudeModifiers.AsReadOnly(); } }
    }
}
