using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Object;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;

namespace SoulSmith.Units
{
    public class UnitUIMoveDescriptionDisplay : CanvasObject
    {
        public const string TEXTBOXKEY = "TextBoxes/Moves/InfoPopup";

        // Children
        private CanvasObject _label;

        public UnitUIMoveDescriptionDisplay(
        string resourceKey,
        Position position = null,
        IEnumerable<SoulSmithObject> children = null) :
        base(
            resourceKey,
            position,
            children)
        {
            InitializeLabel();
        }

        public void UpdateDescription(string description)
        {
            _label.UpdateResourceState(description);
        }

        private void InitializeLabel()
        {
            IDrawableResource textResource = DrawHelpers.GetDrawableResourceInstance(TEXTBOXKEY);
            _label = new CanvasObject(new Position(0, 0, 1, 1, 0, 1), textResource);
            _label.SetColor(new Color(UnitUIMoveButton.LABELBRIGHTNESS, UnitUIMoveButton.LABELBRIGHTNESS, UnitUIMoveButton.LABELBRIGHTNESS, 255));
            AddChild(_label);
            _label.UpdateResourceState("0");
        }
    }
}
