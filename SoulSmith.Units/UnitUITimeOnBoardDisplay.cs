
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Object.Canvas;
using SoulSmith.Asset;

namespace SoulSmith.Units;
public class UnitUITimeOnBoardDisplay : CanvasObject
{
    public const int ROUNDSONBOARDCOUNTERY = -90;

    public UnitUITimeOnBoardDisplay(
        IDrawableResource textResource) : base( 
            new Position(0, -70),
            textResource)
    {}
}

