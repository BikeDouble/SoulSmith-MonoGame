
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Drawing;

public class UnitUITimeOnBoardDisplay : CanvasItem
{
    public const int ROUNDSONBOARDCOUNTERY = -90;

    public UnitUITimeOnBoardDisplay(
        SpriteFont font) : base(
            font, 
            null, 
            new CanvasPosition(0, -70))
    {}
}

