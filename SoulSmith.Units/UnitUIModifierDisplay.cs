
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using SoulSmith.Asset;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Units; 
public class UnitUIModifierDisplay : CanvasObject
{
    public const int ICONSIZE = 40;
    public const int ICONSPERLINE = 5;
    public const int NUMBEROFROWS = 3;
    public const int SPACEBETWEENICONS = 5;
    public const int FIRSTICONX = -(((ICONSIZE * (ICONSPERLINE - 1)) / 2) + (SPACEBETWEENICONS * ((ICONSPERLINE - 1) / 2)));
    public const int FIRSTICONY = 75;
    public static ReadOnlyCollection<Vector2> IconPositions = null;

    // Children
    private Dictionary<IReadOnlyModifier, UnitUIModifierIcon> _displayedIcons;

    public UnitUIModifierDisplay() 
    {
        _displayedIcons = new Dictionary<IReadOnlyModifier, UnitUIModifierIcon>();

        if (IconPositions == null)
        {
            IconPositions = GenerateIconPositions();
        }
    }

    public void UpdateText()
    {
        foreach (var icon in _displayedIcons)
        {
            icon.Value.UpdateText(icon.Key);
        }
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

    public void OnModifierRemoved(IReadOnlyModifier modifier)
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
        //TODO shift modifier icons
    }

    public void OnModifierAdded(IReadOnlyModifier modifier)
    {
        if (modifier.IsVisible)
        {
            bool addedSuccessfully = TryAddDisplayIcon(modifier);
        }
    }

    private UnitUIModifierIcon CreateModifierIcon(IReadOnlyModifier modifier)
    {
        if (modifier == null || !modifier.IsVisible) return null;

        return new UnitUIModifierIcon(new Core.Position(0, 0), modifier);
    }

    private bool TryAddDisplayIcon(IReadOnlyModifier modifier)
    {
        UnitUIModifierIcon addedIcon = CreateModifierIcon(modifier);

        if (addedIcon == null) return false;

        if (_displayedIcons.ContainsKey(modifier)) return false;

        int iconPositionIndex = _displayedIcons.Count;

        _displayedIcons.Add(modifier, addedIcon);

        AddChild(addedIcon);

        if ((iconPositionIndex >= 0) && (iconPositionIndex < IconPositions.Count))
        {
            addedIcon.Translate(IconPositions[iconPositionIndex]);
        }
        else
        {
            addedIcon.Hide(); //TODO add overflow handling
        }

        return true;
    }
}

