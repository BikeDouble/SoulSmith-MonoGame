using SoulSmith.Object.Canvas;
using SoulSmith.Drawing.Zoned;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Units;

namespace SoulSmith.Game
{
    public class GameHeaderUnitInventoryUIButton : ButtonObject
    {
        public static string UNITINVENTORYBUTTONIDLERESOURCEKEY = "ZonedResources/UI/Units/Moves/MoveButton";
        public static string UNITINVENTORYBUTTONHOVEREDRESOURCEKEY = "ZonedResources/UI/Units/Moves/MoveButton";

        public GameHeaderUnitInventoryUIButton(Position position)
            : base(
                  DrawHelpers.GetDrawableResourceInstance(UNITINVENTORYBUTTONIDLERESOURCEKEY) as ZonedDrawableResourceInstance, 
                  DrawHelpers.GetDrawableResourceInstance(UNITINVENTORYBUTTONHOVEREDRESOURCEKEY) as ZonedDrawableResourceInstance, 
                  position)
        {

        }
    }
}
