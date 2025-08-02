using System.Collections.ObjectModel;
using SoulSmith.Object.Canvas;
using SoulSmith.Shapes;
using SoulSmith.UnitStats;
using SoulSmith.Battle.Moves;
using SoulSmith.Drawing;

namespace SoulSmith.Battle;

public interface IReadOnlyUnit : IReadOnlyCanvasObject
{
    ReadOnlyDictionary<StatType, int> StatsList { get; }
    bool InCombat { get; }
    int GetModStat(StatType stat);
    int GetBaseStat(StatType stat);
    ReadOnlyCollection<Moves.Move> MoveSet { get; }
    string SpriteKey { get; }
    IReadOnlyUnitStats ReadOnlyStats { get; }
    IReadOnlyUnitSprite ReadOnlySprite { get; }
    string FriendlyName { get; }
    int MaxHealth { get; }
    int CurHealth { get; }
    int Attack { get; }
    int Defense { get; }
    int CurDecay { get; }
    int DecayRate { get; }
    int TimeOnBoard { get; }
}
