using SoulSmith.Battle;

namespace SoulSmith.Battle.Moves;

public struct MoveInput
{
    public Move Move;
    public IReadOnlyUnit Sender;
    public IReadOnlyUnit Target;
}

