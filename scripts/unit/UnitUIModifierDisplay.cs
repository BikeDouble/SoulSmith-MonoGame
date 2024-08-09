
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using SoulSmithModifiers;

namespace SoulSmithUnitUI;
public class UnitUIModifierDisplay : CanvasItem
{
    public const int ICONSIZE = 30;
    public const int ICONSPERLINE = 5;
    public const int NUMBEROFROWS = 3;
    public const int SPACEBETWEENICONS = 5;
    public const int FIRSTICONX = -(((ICONSIZE * (ICONSPERLINE - 1)) / 2) + (SPACEBETWEENICONS * ((ICONSPERLINE - 1) / 2)));
    public const int FIRSTICONY = 60;

    public static ReadOnlyCollection<Vector2> IconPositions = GenerateIconPositions();

    // Children
    private Dictionary<Modifier, CanvasItem> _displayedIcons;

    public UnitUIModifierDisplay() 
    {
        _displayedIcons = new Dictionary<Modifier, CanvasItem>();
    }

    public static ReadOnlyCollection<Vector2> GenerateIconPositions()
    {
        List<Vector2> iconPositions = new List<Vector2>();

        for (int i = 0; i <= (ICONSPERLINE * NUMBEROFROWS) - 1; i++)
        {
            int row = i / ICONSPERLINE;
            int column = i % ICONSPERLINE;

            Vector2 iconPosition = new Vector2(
                FIRSTICONX + (column * ICONSIZE) + (column * SPACEBETWEENICONS), 
                FIRSTICONY - (row * ICONSIZE) - (row * SPACEBETWEENICONS));

            iconPositions.Add(iconPosition);
        }

        return iconPositions.AsReadOnly();
    }

    public void OnModifierRemoved(Modifier modifier)
    {
        CanvasItem removedIcon = _displayedIcons.GetValueOrDefault(modifier);

        if (removedIcon != null)
        {
            _displayedIcons.Remove(modifier);
            RemoveDisplayIcon(removedIcon);
        }
    }

    private void RemoveDisplayIcon(CanvasItem removedIcon)
    {
        RemoveChild(removedIcon);
        //TODO
    }

    public void OnModifierAdded(Modifier modifier)
    {
        CanvasItem addedIcon = modifier.Icon as CanvasItem;

        if (addedIcon != null)
        {
            if (_displayedIcons.TryAdd(modifier, addedIcon))
                AddDisplayIcon(addedIcon);
        }
    }

    private void AddDisplayIcon(CanvasItem addedIcon)
    {
        int positionIndex = _displayedIcons.Count - 1;

        AddChild(addedIcon);

        if ((positionIndex >= 0) && (positionIndex < IconPositions.Count))
        {
            addedIcon.Translate(IconPositions[positionIndex]);
        }
        else
        {
            addedIcon.Hide();
        }
    }
}

