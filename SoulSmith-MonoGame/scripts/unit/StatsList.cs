using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmithStats;
using SoulSmithMoves;

public class StatsList
{
    private Dictionary<StatType, int> _stats;

    public StatsList(
        int maxHealth = 1000,
        int attack = 100,
        int defense = 100,
        int decayRate = StatConstants.STANDARDDECAYRATE,
        int curHealth = 1000,
        int curDecay = 0)
    {
        _stats = new Dictionary<StatType, int>();
        _stats.Add(StatType.MaxHealth, maxHealth);
        _stats.Add(StatType.CurHealth, curHealth);
        _stats.Add(StatType.Attack, attack);
        _stats.Add(StatType.Defense, defense);
        _stats.Add(StatType.DecayRate, decayRate);
        _stats.Add(StatType.CurDecay, curDecay);
    }

    public StatsList(Dictionary<StatType, int> stats)
    {
        _stats = stats;
    }

    public StatsList(ReadOnlyDictionary<StatType, int> stats)
    {
        _stats = new Dictionary<StatType, int>(stats);
    }

    public int this[StatType statType]
    {
        get { return _stats.GetValueOrDefault(statType); }

        set 
        { 
            if (_stats.ContainsKey(statType))
            {
                _stats[statType] = value;
            }
            else 
            {
                _stats.Add(statType, value);
            }
        }
    }

    public ReadOnlyDictionary<StatType, int> StatsDict { get { return new ReadOnlyDictionary<StatType, int>(_stats);} }
}
