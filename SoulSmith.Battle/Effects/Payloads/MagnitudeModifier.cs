using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Payloads
{
    public class MagnitudeModifier : IStyledNumberModifier
    {
        public IEffectOriginator Originator { get; }
        public StatModStyle ModStyle { get; }
        public float ModAmount { get; }
    }
}
