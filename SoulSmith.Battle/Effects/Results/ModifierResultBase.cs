using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Results
{
    public class ModifierResultBase : ResultBase
    {
        public ModifierResultBase(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyModifier modifier, ResultBase parentResult, PayloadBase payload, IEffectOriginator originator)
            : base(sender, target, parentResult, payload, originator)
        {
            Modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
        }

        public IReadOnlyModifier Modifier { get; }
    }
}
