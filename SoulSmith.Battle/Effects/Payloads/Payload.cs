using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Modifiers;
using System.Collections.ObjectModel;
using SoulSmith.Core;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Battle.Effects.Payloads;
public class Payload
{
    public Payload(IReadOnlyUnit sender, IReadOnlyUnit target, Result parentResult, IEnumerable<IEffect> immediateAfterEffects = null)
    {
        Sender = sender;
        Target = target;
        ParentResult = parentResult;
        if (immediateAfterEffects != null && immediateAfterEffects.Count() > 0)
            ImmediateAfterEffects = new ReadOnlyCollection<IEffect>(immediateAfterEffects.ToList());
        else
            ImmediateAfterEffects = null;
    }

    public IReadOnlyUnit Sender { get; }
    public IReadOnlyUnit Target { get; }
    public Result ParentResult { get; }
    public IReadOnlyCollection<IEffect> ImmediateAfterEffects { get; }
}
