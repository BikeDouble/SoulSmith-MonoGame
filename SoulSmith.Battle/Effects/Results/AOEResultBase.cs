using SoulSmith.Battle.Effects.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class AOEResultBase<T> : ResultBase where T: ResultBase
    {
        public AOEResultBase(IReadOnlyUnit sender, IReadOnlyUnit primaryTarget, IReadOnlyCollection<T> individualResults, ResultBase parentResult, PayloadBase payload, IEffectOriginator originator)
            : base(sender, primaryTarget, parentResult, payload, originator)
        {
            IndividualResults = individualResults;
        }
        public T GetResultOfPrimaryTarget()
        {
            return GetResultOfTarget(Target);
        }
        public T GetResultOfTarget(IReadOnlyUnit target)
        {
            if (target == null) return default;
            return IndividualResults.FirstOrDefault(result => result.Target == target);
        }

        public ICollection<IReadOnlyUnit> GetAllTargets()
        {
            List<IReadOnlyUnit> allTargets = new List<IReadOnlyUnit>();

            foreach (var result in IndividualResults)
            {
                if (result.Target != null && !allTargets.Contains(result.Target))
                {
                    allTargets.Add(result.Target);
                }
            }

            return allTargets;
        }

        public bool ContainsTarget(IReadOnlyUnit target)
        {
            return IndividualResults.Any(result => result.Target == target);
        }

        public IReadOnlyCollection<T> IndividualResults { get; }
    }
}
