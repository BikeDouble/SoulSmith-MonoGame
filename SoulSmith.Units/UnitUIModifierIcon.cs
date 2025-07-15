using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;

namespace SoulSmith.Units
{
    public class UnitUIModifierIcon : CanvasObject
    {
        public UnitUIModifierIcon(Position position, IAssetWrapper<IDrawableResource> wrappedResource) : base(position, wrappedResource)
        {
            ScaleToSetSize(new Vector2(UnitUIModifierDisplay.ICONSIZE, UnitUIModifierDisplay.ICONSIZE), true);
        }
    }
}
