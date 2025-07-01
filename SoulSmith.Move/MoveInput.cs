using SoulSmith.Battle;

namespace SoulSmith.Move
{
    public struct MoveInput
    {
        public Move Move;
        public IReadOnlyUnit Sender;
        public IReadOnlyUnit Target;
    }
}

