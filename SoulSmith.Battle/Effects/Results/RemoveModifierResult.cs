using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class RemoveModifierResult : ModifierResultBase
    {
        public RemoveModifierResult(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyModifier modifier, bool removedSuccessfully, ResultBase parentResult, PayloadBase payload, IEffectOriginator originator)
            : base(sender, target, modifier, parentResult, payload, originator)
        {
            RemovedSuccessfully = removedSuccessfully;
        }

        public bool RemovedSuccessfully { get; }
    }
}
