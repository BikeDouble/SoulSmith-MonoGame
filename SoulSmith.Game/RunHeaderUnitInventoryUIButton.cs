using SoulSmith.Object.Canvas;
using SoulSmith.Drawing.Zoned;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Units;

namespace SoulSmith.Runs
{
    public class RunHeaderUnitInventoryUIButton : ButtonObject
    {
        public static string UNITINVENTORYBUTTONIDLERESOURCEKEY = "ZonedResources/UI/Units/Moves/MoveButton";
        public static string UNITINVENTORYBUTTONHOVEREDRESOURCEKEY = "ZonedResources/UI/Units/Moves/MoveButton";

        public RunHeaderUnitInventoryUIButton(Position position)
            : base(
                  DrawHelpers.GetDrawableResourceInstance(UNITINVENTORYBUTTONIDLERESOURCEKEY) as ZonedDrawableResourceInstance, 
                  DrawHelpers.GetDrawableResourceInstance(UNITINVENTORYBUTTONHOVEREDRESOURCEKEY) as ZonedDrawableResourceInstance, 
                  position)
        {

        }
    }
}
