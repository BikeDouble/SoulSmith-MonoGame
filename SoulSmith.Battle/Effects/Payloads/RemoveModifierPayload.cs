using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class RemoveModifierPayload : Payload
    {
        public RemoveModifierPayload(IReadOnlyUnit sender, IReadOnlyUnit target, IModifier modifier, Result parentResult, IReadOnlyEffect generatingEffect, IEffectOriginator originator, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, generatingEffect, originator, immediateAfterEffects)
        {
            Modifier = modifier;
        }

        public IModifier Modifier { get; }
    }
}
