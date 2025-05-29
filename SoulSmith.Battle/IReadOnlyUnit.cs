using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle
{
    public interface IReadOnlyUnit
    {
        ReadOnlyDictionary<StatType, int> StatsList { get; }
        bool InCombat { get; }
        int GetModStat(StatType stat);
        int GetBaseStat(StatType stat);
        ReadOnlyCollection<Move.Move> MoveSet { get; }
        string FriendlyName { get; }
    }
}
