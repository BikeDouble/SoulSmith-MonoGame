using Microsoft.Xna.Framework;
using SoulSmith.Battle;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Input;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Game;
public class GameHeaderUI : CanvasObject
{
    public const string BACKBOARDRESOURCEKEY = "Textures/UI/Units/List/EntryBackboardIdle";
    public const int HEIGHT = 120;

    private bool _ignoreUnitListUIClickedOutside = false;

    // Children
    private CanvasObject _backboard;
    private GameHeaderUnitInventoryUIButton _unitInventoryButton;
    private UnitListUI _unitListUI;

    public const int INVENTORYBUTTONPOSITIONX = 1000;
    public const int INVENTORYBUTTONPOSITIONY = 50;
    public const float INVENTORYBUTTONSCALE = 0.2f;

    public GameHeaderUI() 
    {
        Initialize();
    }

    private void Initialize()
    {
        InitializeBackboard();
        InitializeUnitInventoryButton();
        InitializeUnitListUI();
    }

    private void InitializeBackboard()
    {
        _backboard = new CanvasObject(BACKBOARDRESOURCEKEY, new Position(0, 0, 1, 1, 0, -1));
        _backboard.ScaleToSetSize(new Vector2(Window.WINDOWLENGTH, HEIGHT), false);
        _backboard.SetOriginPlacement(OriginPlacement.TopLeft);
        AddChild(_backboard);
    }

    private void InitializeUnitInventoryButton()
    {
        _unitInventoryButton = new GameHeaderUnitInventoryUIButton(new Position(INVENTORYBUTTONPOSITIONX, INVENTORYBUTTONPOSITIONY, INVENTORYBUTTONSCALE, INVENTORYBUTTONSCALE));
        AddChild(_unitInventoryButton);
        _unitInventoryButton.ButtonPressedEventHandler += OnUnitInventoryButtonPressed;
    }

    private void InitializeUnitListUI()
    { 
        _unitListUI = new UnitListUI(new Position((Window.WINDOWLENGTH / 2) - (UnitListUI.WIDTH / 2), HEIGHT));
        _unitListUI.ClickedOutsideEventHandler += OnUnitListUIClickedOutside;
        _unitListUI.EntryPressedEventHandler += OnOnUnitListUIEntryPressed;
        _unitListUI.Hide();
        AddChild(_unitListUI);
    }

    public EventHandler<ButtonPressedEventArgs> UnitListUIClickedOutsideEventHandler;

    private void OnUnitListUIClickedOutside(object sender, ButtonPressedEventArgs e)
    {
        if (!_ignoreUnitListUIClickedOutside) _unitListUI.Hide();
    }

    public EventHandler<UnitInventoryButtonPressedEventArgs> UnitInventoryButtonPressedEventHandler;

    // Tells the GameManager that the unit inventory button was pressed.
    private void OnUnitInventoryButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        UnitInventoryButtonPressedEventHandler?.Invoke(this, new UnitInventoryButtonPressedEventArgs()); 
    }

    public EventHandler<UnitListUIEntryPressedEventArgs> UnitListUIEntryPressedEventHandler;

    private void OnOnUnitListUIEntryPressed(object sender, UnitListUIEntryPressedEventArgs args)
    {
        UnitListUIEntryPressedEventHandler?.Invoke(this, args);
    }

    public void ShowUnitInventory(List<IReadOnlyUnit> units)
    {
        _unitListUI.ShowUnits(units);
        _ignoreUnitListUIClickedOutside = true; // Ignore the next ClickedOutside event from the UnitListUI, since it will be triggered by the button press.
    }

    public void HideUnitInventory()
    {
        _unitListUI.Clear();
        _unitListUI.Hide();
    }

    public override void CollectInputPackets(IReadOnlyPosition parentAbsolutePosition, IAddOnly<InputPacket> inputQueue)
    {
        base.CollectInputPackets(parentAbsolutePosition, inputQueue);
        _ignoreUnitListUIClickedOutside = false; // Reset the ignore flag for the next frame.
    }
}

public class UnitInventoryButtonPressedEventArgs : EventArgs
{
    
}

