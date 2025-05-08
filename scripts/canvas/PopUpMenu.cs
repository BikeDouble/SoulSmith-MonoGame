using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmithInput;

/// <summary>
/// CanvasItem that hides itself if clicked outside of.
/// </summary>
public class PopUpMenu : CanvasItem
{
    public PopUpMenu() : base() { }

    public PopUpMenu(
        CanvasPosition position = null,
        DrawableResource sprite = null,
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = null,
        IEnumerable<SoulSmithObject> children = null) : base(position, sprite, boundingZones, children)
    {}

    public override void Process(double delta)
    {
        if (Visible)
        {
            if (!IsMouseOver())
            {
                if (MouseFunctions.IsMouseLeftPressed())
                {
                    Hide();
                }
            }
        }

        base.Process(delta);
    }
}

