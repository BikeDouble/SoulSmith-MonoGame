using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;
using SoulSmith.Drawing.Text;
using SoulSmith.Battle.Modifiers;

namespace SoulSmith.Units
{
    public class UnitUIModifierIcon : CanvasObject
    {
        public readonly static Vector2 DURATIONTEXTPOSITION = new Vector2(UnitUIModifierDisplay.ICONSIZE / 2);
        public readonly static Vector2 TEXTSIZE = new Vector2(UnitUIModifierDisplay.ICONSIZE * 3 / 4);
        public readonly static Vector2 STATUSTEXTPOSITION = new Vector2(DURATIONTEXTPOSITION.X, DURATIONTEXTPOSITION.Y - UnitUIModifierDisplay.ICONSIZE / 4 - GAPBETWEENDURATIONANDSTATUSTEXT);
        public const int GAPBETWEENDURATIONANDSTATUSTEXT = UnitUIModifierDisplay.ICONSIZE / 16;

        public readonly bool ShowingDuration;

        // Children
        private CanvasObject _modifierIcon;
        private CanvasObject _statusTextDisplay;
        private CanvasObject _durationTextDisplay;

        public UnitUIModifierIcon(Position position, IReadOnlyModifier modifier) : base(position)
        {
            UnMirrorable = true;

            ShowingDuration = modifier.DurationStyle != DurationStyle.Permanent;
            InitializeIcon(modifier);
            InitializeStatusTextDisplay(modifier);
            InitializeDurationTextDisplay(modifier);
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
