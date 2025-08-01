using SoulSmith.Core;
using SoulSmith.Object.Canvas;
using SoulSmith.Battle;
using SoulSmith.Drawing;
using Microsoft.Xna.Framework;

namespace SoulSmith.Game;

public class UnitListUIEntry : CanvasObject
{
    public const int WIDTH = 200;
    public const int HEIGHT = 100;
    public const int PADDING = 10;
    public const string IDLEBACKBOARDKEY = "ZonedResources/UI/Units/List/EntryBackboardIdle";
    public const string HOVEREDBACKBOARDKEY = "ZonedResources/UI/Units/List/EntryBackboardHovered";

    private IReadOnlyUnit _unit;

    // Children
    private ButtonObject _backboard;
    private CanvasObject _unitDisplaySprite;

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
        _unitDisplaySprite = new CanvasObject(new Position(PADDING + spriteWidth / 2, PADDING + spriteWidth / 2, 1, 1, 0, 1),
            spriteResource);
        _unitDisplaySprite.ScaleToSetSize(new Vector2(spriteWidth, spriteWidth));
        AddChild(_unitDisplaySprite);
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

