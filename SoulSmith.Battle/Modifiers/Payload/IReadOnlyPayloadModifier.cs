using SoulSmith.Battle.Modifiers.Effect;
using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers.Payload
{
    public interface IReadOnlyPayloadModifier : IReadOnlyModAmountModifier
    {
        public EffectModifierTriggerStyle TriggerStyle { get; }
        public float ModAmount { get; }
        public StatModStyle ModStyle { get; }
    }
}
