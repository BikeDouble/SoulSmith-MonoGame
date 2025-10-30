using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Object;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;

namespace SoulSmith.Units
{
    public class UnitUIModifierDescriptionDisplay : CanvasObject 
    {
        public const string TEXTBOXKEY = "TextBoxes/Modifiers/InfoPopupDescription";

        // Children
        private CanvasObject _descriptionText;

        public UnitUIModifierDescriptionDisplay(
        string fontKey,
        Position position = null,
        IEnumerable<SoulSmithObject> children = null) :
        base(
            fontKey,
            position,
            children)
        {
            InitializeDescriptionText();
        }

        public void UpdateDescription(string description)
        {
            _descriptionText.UpdateResourceState(description);
        }

        private void InitializeDescriptionText()
        {
            IDrawableResource textResource = DrawHelpers.GetDrawableResourceInstance(TEXTBOXKEY);
            _descriptionText = new CanvasObject(new Position(0, 0, 1, 1, 0, 1), textResource);
            AddChild(_descriptionText);
            _descriptionText.UpdateResourceState("0");
            _descriptionText.SetColor(Color.Black);
        }

        public override void SetOriginPlacement(OriginPlacement originPlacement)
        {
            base.SetOriginPlacement(originPlacement);

            _descriptionText.SetOriginPlacement(originPlacement);
        }
    }
}
