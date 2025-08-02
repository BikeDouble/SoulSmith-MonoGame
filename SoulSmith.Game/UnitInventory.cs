
using SoulSmith.Units;
using SoulSmith.Object.Canvas;
using SoulSmith.Battle;

namespace SoulSmith.Game;
public class UnitInventory : CanvasObject
{
    private List<Unit> _units = new List<Unit>();

    /// <summary>
    /// Adds a unit to the inventory, does not allow duplicates.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns> True if unit successfully added, false otherwise.</returns>
    public bool AddUnit(Unit unit)
    {
        if (_units.Contains(unit)) return false;

        if (unit == null) return false;

        _units.Add(unit);
        return true;
    }

    /// <summary>
    /// Remove given unit from the inventory.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns>True if unit successfully found and removed, false otherwise.</returns>
    public bool RemoveUnit(Unit unit)
    {
        return _units.Remove(unit);
    }

    /// <summary>
    /// Gets a copy of the list of units in the inventory.
    /// </summary>
    /// <returns></returns>
    public List<Unit> GetUnits() {
        return new List<Unit>(_units); 
    }

    /// <summary>
    /// Gets a copy of the list of units in the inventory as readonly.
    /// </summary>
    /// <returns></returns>
    public List<IReadOnlyUnit> GetUnitsAsReadOnly()
    {
        return new List<IReadOnlyUnit>(_units);
    }

    public Unit GetMatchingUnit(IReadOnlyUnit readOnlyUnit)
    {
        foreach (Unit unit in _units)
        {
            if (unit == readOnlyUnit) return unit;
        }

        return null;
    }
}

