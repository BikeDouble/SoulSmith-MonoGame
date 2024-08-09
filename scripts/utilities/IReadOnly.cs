using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using SoulSmithMoves;
using SoulSmithStats;


public interface IReadOnlyModifier
{

}

public interface IReadOnlySoulSmithObject : IDeepCloneable
{

}

public interface IReadOnlyUnitStats : IReadOnlySoulSmithObject
{
    int GetModStat(StatType stat);
    int GetBaseStat(StatType stat);
    int CombatPosition { get; }
    int TimeOnBoard { get; }
}

public interface IReadOnlyCanvasItem : IReadOnlySoulSmithObject
{
    Vector2 GetRandomBoundingPointLocal(BoundingZoneType type);
    Vector2 GetRandomBoundingPointGlobal(BoundingZoneType type);
}

public interface IReadOnlyUnit : IReadOnlyCanvasItem
{
    ReadOnlyDictionary<StatType, int> StatsList { get; }
    bool InCombat { get; }
    int GetModStat(StatType stat);
    int GetBaseStat(StatType stat);
    ReadOnlyCollection<Move> MoveSet { get; }
}

