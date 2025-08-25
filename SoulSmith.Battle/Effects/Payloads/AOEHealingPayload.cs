using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class AOEHealingPayload : AOEPayloadBase
    {
        private readonly List<MagnitudeModifier> _allTargetMagnitudeModifiers = new List<MagnitudeModifier>();
        private readonly Dictionary<IReadOnlyUnit, List<MagnitudeModifier>> _targetSpecificMagnitudeModifiers = new Dictionary<IReadOnlyUnit, List<MagnitudeModifier>>();

        public AOEHealingPayload(IReadOnlyUnit sender, IReadOnlyUnit primaryTarget, ICollection<IReadOnlyUnit> secondaryTargets, int rawHealing, float fractionOfHealingToSecondaryTargets, ResultBase parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null)
            : base(sender, primaryTarget, secondaryTargets, parentResult, generatingEffect, originator, immediateAfterEffects)
        {
            RawHealing = rawHealing;
            FractionOfHealingToSecondaryTargets = fractionOfHealingToSecondaryTargets;
        }

        public void AddAllTargetMagnitudeModifier(MagnitudeModifier magnitudeModifier)
        {
            _allTargetMagnitudeModifiers.Add(magnitudeModifier);
        }

        public void AddTargetSpecificMagnitudeModifier(IReadOnlyUnit target, MagnitudeModifier magnitudeModifier)
        {
            if (!_targetSpecificMagnitudeModifiers.ContainsKey(target))
            {
                _targetSpecificMagnitudeModifiers[target] = new List<MagnitudeModifier>();
            }

            _targetSpecificMagnitudeModifiers[target].Add(magnitudeModifier);
        }

        public override List<IEffectOriginator> GetModifyingObjects()
        {
            List<IEffectOriginator> modifyingObjects = base.GetModifyingObjects();

            foreach (var kvp in _targetSpecificMagnitudeModifiers)
            {
                if (kvp.Value != null)
                {
                    foreach (var modifier in kvp.Value)
                    {
                        if (modifier.Originator != null && !modifyingObjects.Contains(modifier.Originator))
                        {
                            modifyingObjects.Add(modifier.Originator);
                        }
                    }
                }
            }

            foreach (var modifier in _allTargetMagnitudeModifiers)
            {
                if (modifier.Originator != null && !modifyingObjects.Contains(modifier.Originator))
                {
                    modifyingObjects.Add(modifier.Originator);
                }
            }

            return modifyingObjects;
        }

        public ReadOnlyCollection<MagnitudeModifier> GetTargetSpecificMagnitudeModifiers(IReadOnlyUnit target)
        {
            if (!_targetSpecificMagnitudeModifiers.ContainsKey(target)) return _allTargetMagnitudeModifiers.AsReadOnly();
            if (_targetSpecificMagnitudeModifiers[target] == null) return _allTargetMagnitudeModifiers.AsReadOnly();

            return _targetSpecificMagnitudeModifiers[target].Concat(_allTargetMagnitudeModifiers).ToList().AsReadOnly();
        }

        public override PayloadBase GetTargetSpecificPayload(IReadOnlyUnit target)
        {
            if (target == null) return null;

            float healingMult;

            if (target == Target)
            {
                healingMult = 1.0f;
            }
            else if (SecondaryTargets.Contains(target))
            {
                healingMult = FractionOfHealingToSecondaryTargets;
            }
            else return null;

            HealingPayload healingPayload = new HealingPayload(Sender,
                    target,
                    (int)(RawHealing * healingMult),
                    ParentResult,
                    GeneratingEffect,
                    Originator,
                    ImmediateAfterEffects);

            foreach (var modifier in GetTargetSpecificMagnitudeModifiers(target))
            {
                healingPayload.AddMagnitudeModifier(modifier);
            }

            return healingPayload;
        }

        public int RawHealing { get; }
        public float FractionOfHealingToSecondaryTargets { get; }
        public ReadOnlyCollection<MagnitudeModifier> AllTargetMagnitudeModifiers { get { return _allTargetMagnitudeModifiers.AsReadOnly(); } }
    }
}
