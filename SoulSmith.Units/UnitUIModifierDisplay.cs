
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using SoulSmith.Battle.Modifier;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Units; 
public class UnitUIModifierDisplay : CanvasObject
{
    public const int ICONSIZE = 30;
    public const int ICONSPERLINE = 5;
    public const int NUMBEROFROWS = 3;
    public const int SPACEBETWEENICONS = 5;
    public const int FIRSTICONX = -(((ICONSIZE * (ICONSPERLINE - 1)) / 2) + (SPACEBETWEENICONS * ((ICONSPERLINE - 1) / 2)));
    public const int FIRSTICONY = 60;

    public static ReadOnlyCollection<Vector2> IconPositions = GenerateIconPositions();

    // Children
    private Dictionary<IModifier, CanvasObject> _displayedIcons;

    public UnitUIModifierDisplay() 
    {
        _displayedIcons = new Dictionary<IModifier, CanvasObject>();
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

    public void OnModifierRemoved(IModifier modifier)
    {
        CanvasObject removedIcon = _displayedIcons.GetValueOrDefault(modifier);

        if (removedIcon != null)
        {
            _displayedIcons.Remove(modifier);
            RemoveDisplayIcon(removedIcon);
        }
    }

    private void RemoveDisplayIcon(CanvasObject removedIcon)
    {
        RemoveChild(removedIcon);
        //TODO
    }

    public void OnModifierAdded(IModifier modifier)
    {
        /*CanvasObject addedIcon = modifier.Icon as CanvasObject; //TODO

        if (addedIcon != null)
        {
            if (_displayedIcons.TryAdd(modifier, addedIcon))
                AddDisplayIcon(addedIcon);
        }*/
    }

    private void AddDisplayIcon(CanvasObject addedIcon)
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

