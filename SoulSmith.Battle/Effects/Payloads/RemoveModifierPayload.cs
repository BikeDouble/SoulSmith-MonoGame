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
        public RemoveModifierPayload(IReadOnlyUnit sender, IReadOnlyUnit target, IModifier modifier, Result parentResult, IEnumerable<IEffect> immediateAfterEffects = null) : base(sender, target, parentResult, immediateAfterEffects)
        {
            Modifier = modifier;
        }

        public IModifier Modifier { get; }
    }
}
