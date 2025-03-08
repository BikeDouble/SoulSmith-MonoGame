
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SoulSmithUnitUI;

using Entry = SoulSmithUnitUI.UnitListUIEntry;

public class UnitListUI : CanvasItem
{
    // Children
    private List<Entry> _entries;

    public UnitListUI()
    {

    }

    public bool AddUnit(Unit unit)
    {
        if (ContainsUnit(unit)) return false;

        Entry entry = new Entry(unit);
        AddChild(entry);
        _entries.Add(entry);
        return true;
    }

    public bool RemoveUnit(Unit unit)
    {
        Entry entry = GetEntryWithUnit(unit);

        if (entry == null) return false;

        return _entries.Remove(entry);
    }

    public void SetEntries(IEnumerable<Unit> units)
    {
        Clear();
        foreach (Unit unit in units)
        {
            AddUnit(unit);
        }
    }

    public void Clear()
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

    public void Show(IEnumerable<Unit> units)
    {
        SetEntries(units);
        Show();
    }

}