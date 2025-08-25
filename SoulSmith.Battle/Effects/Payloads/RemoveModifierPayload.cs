using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class RemoveModifierPayload : ModifierPayloadBase
    {
        public RemoveModifierPayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyModifier modifier, ResultBase parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, modifier, parentResult, generatingEffect, originator, immediateAfterEffects)
        {
        }

    }
}
