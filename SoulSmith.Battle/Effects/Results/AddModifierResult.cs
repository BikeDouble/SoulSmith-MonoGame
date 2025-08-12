using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Modifiers;

namespace SoulSmith.Battle.Effects.Results
{
    public class AddModifierResult : Result
    {
        public AddModifierResult(IReadOnlyUnit sender, IReadOnlyUnit target, IModifier modifier, bool appliedSuccessfully, Result parentResult, Payload payload)
            : base(sender, target, parentResult, payload)
        {
            Modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
            AppliedSuccessfully = appliedSuccessfully;
        }

        public IModifier Modifier { get; }
        public bool AppliedSuccessfully { get; }
    }
}
