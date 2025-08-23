using Microsoft.VisualBasic;
using SoulSmith.Battle.Effects.Results;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class AOEPayloadBase : PayloadBase
    {
        private IReadOnlyCollection<IReadOnlyUnit> _secondaryTargets;

        public AOEPayloadBase(IReadOnlyUnit sender, IReadOnlyUnit primaryTarget, ICollection<IReadOnlyUnit> secondaryTargets, ResultBase parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null)
            : base(sender, primaryTarget, parentResult, generatingEffect, originator, immediateAfterEffects)
        {
            _secondaryTargets = new ReadOnlyCollection<IReadOnlyUnit>(secondaryTargets.ToList());
        }

        public IReadOnlyCollection<IReadOnlyUnit> SecondaryTargets { get { return _secondaryTargets; } }
        public IReadOnlyCollection<IReadOnlyUnit> AllTargets { get { return new List<IReadOnlyUnit> { Target }.Concat(_secondaryTargets).ToList(); } }

        public static ICollection<IReadOnlyUnit> GetSecondaryTargets(IReadOnlyUnit target, IReadOnlyCombat combat, AOETargetStyle targetStyle)
        {
            List<IReadOnlyUnit> secondaryTargets;

            switch (targetStyle)
            {
                case AOETargetStyle.WholeTeam:
                    secondaryTargets = combat.GetReadOnlyTeamWithUnit(target)?.GetReadOnlyUnits().ToList() ?? new List<IReadOnlyUnit>();
                    secondaryTargets.Remove(target); // Remove the primary target from the secondary targets
                    return secondaryTargets;
                case AOETargetStyle.WholeCombat:
                    secondaryTargets = combat.GetAllReadOnlyUnits().ToList();
                    secondaryTargets.Remove(target);
                    return secondaryTargets;
                case AOETargetStyle.Adjacent:
                    return combat.GetReadOnlyTeamWithUnit(target)?.GetAdjacentReadOnlyUnits(target)?.ToList() ?? new List<IReadOnlyUnit>();
                default:
                    throw new ArgumentException("Invalid AOETargetStyle provided.");
            }
        }

        public virtual PayloadBase GetTargetSpecificPayload(IReadOnlyUnit target)
        {
            return null;
        }
    }
}
