using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Modifiers;

namespace SoulSmith.Battle.Effects.Results;
public partial class Result
{
    public Result(IReadOnlyUnit sender, IReadOnlyUnit target, Result parentResult)
    {
        Sender = sender;
        Target = target;
    }

    public IReadOnlyUnit Sender { get; }
    public IReadOnlyUnit Target { get; }
    public Result ParentResult { get; }
}
