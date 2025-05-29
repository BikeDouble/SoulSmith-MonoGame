using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle
{
    public interface IReadOnlyUnitStats
    {
        int GetModStat(StatType stat);
        int GetBaseStat(StatType stat);
        int CombatPosition { get; }
        int TimeOnBoard { get; }
    }
}
