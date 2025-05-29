
using SoulSmith.Units;
using System.Collections.Generic;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Game;
public class UnitInventory : CanvasObject
{
    private List<Unit> _units = new List<Unit>();
    private UnitListUI _uI = new();

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
    /// Show units in the inventory.
    /// </summary>
    public void ShowUnits()
    {
        _uI.ShowUnits(_units);
    }

}

