using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Modifiers;

namespace SoulSmith.Battle.Effects.Results;
public partial class Result
{
    public Result(IReadOnlyUnit sender, IReadOnlyUnit target, Result parentResult, Payload payload)
    {
        Sender = sender;
        Target = target;
        ParentResult = parentResult;
        Payload = payload;
    }

    public IReadOnlyUnit Sender { get; }
    public IReadOnlyUnit Target { get; }
    public Payload Payload { get; }
    public Result ParentResult { get; }
}
