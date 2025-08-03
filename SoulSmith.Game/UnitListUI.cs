
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using SoulSmith.Object.Canvas;
using SoulSmith.Units;
using SoulSmith.Drawing;
using SoulSmith.Core;
using Microsoft.Xna.Framework;
using SoulSmith.Battle;

namespace SoulSmith.Game;

using Entry = UnitListUIEntry;

public class UnitListUI : CanvasObject
{
    public const string BACKBOARDKEY = "ZonedResources/UI/Units/List/Backboard";
    public const int ENTRIESPERPAGE = 5;
    public const int WIDTH = UnitListUIEntry.WIDTH;
    public const int HEIGHT = UnitListUIEntry.HEIGHT * ENTRIESPERPAGE;

    // Children
    private List<Entry> _entries;
    private InverseButton _backboard;

    public UnitListUI(Position position) : base(position)
    {
        _entries = new List<Entry>();
        this.SetOriginPlacement(OriginPlacement.TopLeft);

        _backboard = new InverseButton(new Position(0, 0, 1, 1, 0, -1), BACKBOARDKEY);
        _backboard.SetOriginPlacement(OriginPlacement.TopLeft);
        _backboard.ButtonPressedEventHandler += OnClickedOutside;
        _backboard.ScaleToSetSize(new Vector2(WIDTH, HEIGHT), false);
        AddChild(_backboard);
    }

    private bool AddUnit(IReadOnlyUnit unit)
    {
        Entry entry = new Entry(unit, new Position(0, _entries.Count * Entry.HEIGHT, 1, 1, 0, 1));
        return AddEntry(entry);
    }

    private bool AddEntry(Entry entry)
    {
        AddChild(entry);
        _entries.Add(entry);
        entry.EntryPressedEventHandler += OnEntryPressed;
        return true;
    }

    private void SetEntries(IEnumerable<IReadOnlyUnit> units)
    {
        Clear();
        foreach (IReadOnlyUnit unit in units)
        {
            AddUnit(unit);
        }
    }

    public void Clear()
    {
        foreach (Entry entry in _entries)
        {
            RemoveChild(entry);
            entry.EntryPressedEventHandler -= OnEntryPressed;
            entry.Dispose();
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

    public EventHandler<UnitListUIEntryPressedEventArgs> EntryPressedEventHandler;

    private void OnEntryPressed(object sender, UnitListUIEntryPressedEventArgs args)
    {
        EntryPressedEventHandler?.Invoke(this, args);
    }

    public EventHandler<ButtonPressedEventArgs> ClickedOutsideEventHandler;

    private void OnClickedOutside(object sender, ButtonPressedEventArgs args)
    {
        ClickedOutsideEventHandler?.Invoke(this, args);
    }

    public void ShowUnits(IEnumerable<IReadOnlyUnit> units)
    {
        SetEntries(units);
        Show();
    }
}