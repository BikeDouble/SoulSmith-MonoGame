using SoulSmith.Core;
using SoulSmith.Object.Canvas;
using SoulSmith.Battle;
using SoulSmith.Drawing;
using Microsoft.Xna.Framework;
using SoulSmith.Units;

namespace SoulSmith.Runs;

public class UnitListUIEntry : CanvasObject
{
    public const int WIDTH = 200;
    public const int HEIGHT = 100;
    public const int PADDING = 10;
    public const int PADDINGBETWEENHEALTHBARANDSPRITE = PADDING;
    public readonly static int HEALTHBARWIDTH = (int)(UnitUIHealthBar.FILLERDIMENSIONS.X * (HEIGHT - 2 * PADDING) * (UnitUI.HEALTHBARSCALE.X / UnitUI.HEALTHBARSCALE.Y) / UnitUIHealthBar.FILLERDIMENSIONS.Y);
    public const string IDLEBACKBOARDKEY = "ZonedResources/UI/Units/List/EntryBackboardIdle";
    public const string HOVEREDBACKBOARDKEY = "ZonedResources/UI/Units/List/EntryBackboardHovered";

    private IReadOnlyUnit _unit;

    // Children
    private ButtonObject _backboard;
    private CanvasObject _unitDisplaySprite;
    private UnitUIHealthBar _healthBar;

    public UnitListUIEntry(
        IReadOnlyUnit unit,
        Position position = null) : base(position)
    {
        _unit = unit;

        Initialize();
    }

    private void Initialize()
    {
        InitializeDisplaySprite();
        InitializeBackboard();
        InitializeHealthBar();
    }

    private void InitializeBackboard()
    {
        _backboard = new ButtonObject(IDLEBACKBOARDKEY, HOVEREDBACKBOARDKEY, null);
        _backboard.SetOriginPlacement(OriginPlacement.TopLeft);
        _backboard.ScaleToSetSize(new Vector2(WIDTH, HEIGHT), false);
        _backboard.ButtonPressedEventHandler += OnEntryPressed;
        AddChild(_backboard);
    }

    private void InitializeDisplaySprite()
    {
        IDrawableResource spriteResource = DrawHelpers.GetDrawableResourceInstance(_unit.SpriteKey);
        int spriteWidth = HEIGHT - (2 * PADDING);
        _unitDisplaySprite = new CanvasObject(new Position(PADDINGBETWEENHEALTHBARANDSPRITE + PADDING + spriteWidth / 2 + HEALTHBARWIDTH, PADDING + spriteWidth / 2, 1, 1, 0, 1),
            spriteResource);
        _unitDisplaySprite.ScaleToSetSize(new Vector2(spriteWidth, spriteWidth));
        AddChild(_unitDisplaySprite);
    }

    private void InitializeHealthBar()
    {
        Vector2 trueResourceSize = UnitUIHealthBar.FILLERDIMENSIONS;
        int width = HEALTHBARWIDTH;
        int height = HEIGHT - (2 * PADDING);
        _healthBar = new UnitUIHealthBar(new Position(PADDING + width / 2, PADDING + height / 2, 1, 1, 0, 2 ));
        _healthBar.Update(_unit);
        _healthBar.ScaleToSetSize(new Vector2(width, height), false, trueResourceSize);
        AddChild(_healthBar);
    }

    public EventHandler<UnitListUIEntryPressedEventArgs> EntryPressedEventHandler;

    private void OnEntryPressed(object sender, ButtonPressedEventArgs e)
    {
        UnitListUIEntryPressedEventArgs args = new UnitListUIEntryPressedEventArgs(_unit);
        EntryPressedEventHandler?.Invoke(this, args);
    }

    public override void Dispose()
    {
        base.Dispose();

        _unit = null;
    }

    public IReadOnlyUnit Unit { get { return _unit; } }
}

public class UnitListUIEntryPressedEventArgs : EventArgs
{
    public UnitListUIEntryPressedEventArgs(IReadOnlyUnit unit)
    {
        Unit = unit;
    }

    public IReadOnlyUnit Unit { get; }
}

