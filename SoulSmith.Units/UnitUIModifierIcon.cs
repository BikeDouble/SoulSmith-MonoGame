using Microsoft.Xna.Framework;
using SoulSmith.Asset;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Moves;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Drawing.Text;
using SoulSmith.Object.Canvas;
using SoulSmith.Shapes;

namespace SoulSmith.Units
{
    public class UnitUIModifierIcon : ButtonObject
    {
        public const string HOVERZONEICONKEY = "ZonedResources/UI/Units/Modifiers/IconHoverZone";
        public const string DESCRIPTIONDISPLAYBACKBOARDKEY = "Textures/UI/Units/Modifiers/InfoPopup";
        public const float TIMEHOVEREDFORDESCRIPTIONDISPLAY = 0.2f;
        public readonly static Vector2 DURATIONTEXTPOSITION = new Vector2(UnitUIModifierDisplay.ICONSIZE / 2);
        public readonly static Vector2 TEXTSIZE = new Vector2(UnitUIModifierDisplay.ICONSIZE * 3 / 4);
        public readonly static Vector2 STATUSTEXTPOSITION = new Vector2(DURATIONTEXTPOSITION.X, DURATIONTEXTPOSITION.Y - UnitUIModifierDisplay.ICONSIZE / 4 - GAPBETWEENDURATIONANDSTATUSTEXT);
        public const int GAPBETWEENDURATIONANDSTATUSTEXT = UnitUIModifierDisplay.ICONSIZE / 16;

        public readonly bool ShowingDuration;

        // Children
        private CanvasObject _modifierIcon;
        private CanvasObject _statusTextDisplay;
        private CanvasObject _durationTextDisplay;
        private UnitUIModifierDescriptionDisplay _descriptionDisplay;

        public UnitUIModifierIcon(Position position, IReadOnlyModifier modifier) : base(HOVERZONEICONKEY, HOVERZONEICONKEY, position)
        {
            UnMirrorable = true;

            ShowingDuration = modifier.DurationStyle != DurationStyle.Permanent;
            InitializeIcon(modifier);
            InitializeStatusTextDisplay(modifier);
            InitializeDurationTextDisplay(modifier);
            InitializeDescriptionDisplay(modifier);
        }

        public override void Process(double delta)
        {
            base.Process(delta);

            if (TimeHovered > TIMEHOVEREDFORDESCRIPTIONDISPLAY) _descriptionDisplay.Show();
        }

        public override void OnMouseExit()
        {
            base.OnMouseExit();

            _descriptionDisplay.Hide();
        }

        private void InitializeDescriptionDisplay(IReadOnlyModifier modifier)
        {
            _descriptionDisplay = new UnitUIModifierDescriptionDisplay(DESCRIPTIONDISPLAYBACKBOARDKEY, new Position(new Vector2(UnitUIModifierDisplay.ICONSIZE / 2, 0), new Vector2(0.27f, 0.27f), 0, 10));
            _descriptionDisplay.Hide();
            _descriptionDisplay.UpdateDescription(modifier.Description);
            _descriptionDisplay.SetOriginPlacement(OriginPlacement.LeftMiddle);
            AddChild(_descriptionDisplay);
        }

        private void InitializeIcon(IReadOnlyModifier modifier)
        {
            _modifierIcon = new CanvasObject(modifier.IconKey, new Core.Position(0, 0, 1, 1, 0, -1));
            _modifierIcon.ScaleToSetSize(new Vector2(UnitUIModifierDisplay.ICONSIZE), true);
            AddChild(_modifierIcon);
        }

        private void InitializeStatusTextDisplay(IReadOnlyModifier modifier)
        {
            Vector2 coordinates = ShowingDuration ? STATUSTEXTPOSITION : DURATIONTEXTPOSITION;

            _statusTextDisplay = new CanvasObject(UnitUIMoveButton.LABELFONTKEY, new Position(coordinates, Vector2.One, 0, 1));
            _statusTextDisplay.ScaleToSetSize(TEXTSIZE, true, new Vector2(FontResource.STANDARDFONTSIZE));
            _statusTextDisplay.UpdateResourceState(modifier.StatusText);
            _statusTextDisplay.SetOriginPlacement(OriginPlacement.BottomRight);
            AddChild(_statusTextDisplay);
        }

        private void InitializeDurationTextDisplay(IReadOnlyModifier modifier)
        {
            if (!ShowingDuration) return;

            _durationTextDisplay = new CanvasObject(UnitUIMoveButton.LABELFONTKEY, new Position(DURATIONTEXTPOSITION, Vector2.One, 0, 1));
            _durationTextDisplay.ScaleToSetSize(TEXTSIZE, true, new Vector2(FontResource.STANDARDFONTSIZE));
            _durationTextDisplay.UpdateResourceState(modifier.Duration.ToString());
            _durationTextDisplay.SetOriginPlacement(OriginPlacement.BottomRight);
            AddChild(_durationTextDisplay);
        }

        public void UpdateText(IReadOnlyModifier modifier)
        {
            _statusTextDisplay.UpdateResourceState(modifier.StatusText);
            _durationTextDisplay.UpdateResourceState(modifier.Duration.ToString());
        }
    }
}
