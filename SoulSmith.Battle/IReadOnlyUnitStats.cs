using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Modifiers;
using SoulSmith.UnitStats;

namespace SoulSmith.Battle
{
    public interface IReadOnlyUnitStats : IEffectOriginator
    {
        int GetModStat(StatType stat);
        int GetBaseStat(StatType stat);
        IReadOnlyModifier GetReadOnlyModifier(string mergeKey);
        int CombatPosition { get; }
        int TimeOnBoard { get; }
        int MaxHealth { get; }
        int CurHealth { get; }
        int Attack { get; }
        int Defense { get; }
        int CurDecay { get; }
        int DecayRate { get; }
    }
}
