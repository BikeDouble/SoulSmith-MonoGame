

namespace SoulSmith.Battle.Modifiers
{
    public interface IReadOnlyModifier
    {
        public string IconKey { get; }
        public int Duration { get; }
        public DurationStyle DurationStyle { get; }
        public string StatusText { get; }
        public bool IsVisible { get; }
    }
}
