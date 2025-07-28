
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SoulSmith.Object.Canvas;
using SoulSmith.Units;

namespace SoulSmith.Game;

using Entry = UnitListUIEntry;

public class UnitListUI : PopUpMenu
{
    public const int ENTRIESPERPAGE = 5;
    public const int WIDTH = UnitListUIEntry.WIDTH;
    public const int HEIGHT = UnitListUIEntry.HEIGHT * ENTRIESPERPAGE;

    // Children
    private List<Entry> _entries;

    public UnitListUI() : base(null, WIDTH, HEIGHT)
    {
        _entries = new List<Entry>();
    }

    private bool AddUnit(Unit unit)
    {
        if (ContainsUnit(unit)) return false;

        Entry entry = new Entry(unit);
        AddChild(entry);
        _entries.Add(entry);
        return true;
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
}