

namespace SoulSmith.Battle.Modifiers
{
    public interface IReadOnlyModifier
    {
        public string IconKey { get; }
        public int Duration { get; }
        public DurationStyle DurationStyle { get; }
        public ModifierAlignment Alignment { get; }
        public string StatusText { get; }
        public string FriendlyName { get; }
        public string Description { get; }
        public string MergeKey { get; }
        public bool IsVisible { get; }
    }
}
