using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Core;
using SoulSmith.UnitStats;
using System.Collections.ObjectModel;

namespace SoulSmith.Battle.Effects.Payloads;
public class PayloadBase
{
    public PayloadBase(IReadOnlyUnit sender, IReadOnlyUnit target, ResultBase parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null)
    {
        Sender = sender;
        Target = target;
        ParentResult = parentResult;
        GeneratingEffect = generatingEffect;
        Originator = originator;
        if (immediateAfterEffects != null && immediateAfterEffects.Count() > 0)
            ImmediateAfterEffects = new ReadOnlyCollection<IEffect>(immediateAfterEffects.ToList());
        else
            ImmediateAfterEffects = null;
    }

    public IReadOnlyUnit Sender { get; }
    public IReadOnlyUnit Target { get; }
    public IReadOnlyEffect GeneratingEffect { get; }
    public IEffectOriginator Originator { get; }
    public ResultBase ParentResult { get; }
    public IReadOnlyCollection<IEffect> ImmediateAfterEffects { get; }

    public virtual List<IEffectOriginator> GetModifyingObjects()
    {
        return new List<IEffectOriginator>();
    }
}
