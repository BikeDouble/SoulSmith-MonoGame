using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Modifiers;

namespace SoulSmith.Battle.Effects.Results
{
    public class AddModifierResult : ModifierResultBase
    {
        public AddModifierResult(IReadOnlyUnit sender, IReadOnlyUnit target, IModifier modifier, bool appliedSuccessfully, ResultBase parentResult, PayloadBase payload, IEffectOriginator originator)
            : base(sender, target, modifier, parentResult, payload, originator)
        {
            AppliedSuccessfully = appliedSuccessfully;
        }

        public bool AppliedSuccessfully { get; }
    }
}
