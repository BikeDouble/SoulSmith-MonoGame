
using Microsoft.Xna.Framework.Graphics;

public class UnitUITimeOnBoardDisplay : CanvasItem
{
    public const int ROUNDSONBOARDCOUNTERY = -90;

    public UnitUITimeOnBoardDisplay(
        SpriteFont font) : base(
            font, 
            null, 
            new Position(0, -70))
    {}
}

