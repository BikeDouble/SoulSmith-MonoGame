using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers
{
    public interface IReadOnlyStatModifier : IReadOnlyModifier
    {
        public StatType StatType { get; }
        public float ModAmount { get; }
        public StatModStyle ModStyle { get; }
    }
}
