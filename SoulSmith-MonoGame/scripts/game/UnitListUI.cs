
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SoulSmithUnitUI;

using Entry = SoulSmithUnitUI.UnitListUIEntry;

public class UnitListUI : CanvasObject
{
    // Children
    private List<Entry> _entries;

    public UnitListUI()
    {

    }

    private bool AddUnit(Unit unit)
    {
        if (ContainsUnit(unit)) return false;

        Entry entry = new Entry(unit);
        AddChild(entry);
        _entries.Add(entry);
        return true;
    }

    private bool RemoveUnit(Unit unit)
    {
        Entry entry = GetEntryWithUnit(unit);

        if (entry == null) return false;

        return _entries.Remove(entry);
    }

    private void SetEntries(IEnumerable<Unit> units)
    {
        Clear();
        foreach (Unit unit in units)
        {
            AddUnit(unit);
        }
    }

    private void Clear()
    {
        foreach (Entry entry in _entries)
        {
            RemoveChild(entry);
        }

        _entries.Clear();
    }

    public bool ContainsUnit(Unit unit)
    {
        foreach (var entry in _entries)
        {
            if (entry.Unit == unit)
                return true;
        }
        return false;
    }

    private Entry GetEntryWithUnit(Unit unit)
    {
        foreach (var entry in _entries)
        {
            if (entry.Unit == unit)
                return entry;
        }
        return null;
    }

    public void ShowUnits(IEnumerable<Unit> units)
    {
        SetEntries(units);
        Show();
    }

    public void HideUnits()
    {
        Hide();
        Clear();
    }
       

}