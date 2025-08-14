using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class RemoveModifierResult : Result
    {
        public RemoveModifierResult(IReadOnlyUnit sender, IReadOnlyUnit target, IModifier modifier, bool removedSuccessfully, Result parentResult, Payload payload, IEffectOriginator originator)
            : base(sender, target, parentResult, payload, originator)
        {
            Modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
            RemovedSuccessfully = removedSuccessfully;
        }

        public IModifier Modifier { get; }
        public bool RemovedSuccessfully { get; }
    }
}
