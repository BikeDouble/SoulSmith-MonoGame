using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Modifiers;

namespace SoulSmith.Battle.Effects.Results;
public partial class ResultBase
{
    public ResultBase(IReadOnlyUnit sender, IReadOnlyUnit target, ResultBase parentResult, PayloadBase payload, IEffectOriginator originator)
    {
        Sender = sender;
        Target = target;
        ParentResult = parentResult;
        Payload = payload;
        Originator = originator;
    }

    public IReadOnlyUnit Sender { get; }
    public IReadOnlyUnit Target { get; }
    public IEffectOriginator Originator { get; }
    public PayloadBase Payload { get; }
    public ResultBase ParentResult { get; }
}
