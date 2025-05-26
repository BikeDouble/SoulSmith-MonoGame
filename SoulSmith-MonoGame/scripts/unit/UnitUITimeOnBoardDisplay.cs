
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Drawing;
using SoulSmith.Core;

public class UnitUITimeOnBoardDisplay : CanvasObject
{
    public const int ROUNDSONBOARDCOUNTERY = -90;

    public UnitUITimeOnBoardDisplay(
        SpriteFont font) : base(
            font, 
            null, 
            new Position(0, -70))
    {}
}

