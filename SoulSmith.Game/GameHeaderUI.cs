using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Object.Canvas;
using SoulSmith.Drawing.Zoned;
using SoulSmith.Units;
using SoulSmith.Input;
using SoulSmith.Battle;

namespace SoulSmith.Game;
public class GameHeaderUI : CanvasObject
{
    private bool _ignoreUnitListUIClickedOutside = false;

    // Children
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
        InitializeUnitInventoryButton();
        InitializeUnitListUI();
    }

    private void InitializeUnitInventoryButton()
    {
        _unitInventoryButton = new GameHeaderUnitInventoryUIButton(new Position(INVENTORYBUTTONPOSITIONX, INVENTORYBUTTONPOSITIONY, INVENTORYBUTTONSCALE, INVENTORYBUTTONSCALE));
        AddChild(_unitInventoryButton);
        _unitInventoryButton.ButtonPressedEventHandler += OnUnitInventoryButtonPressed;
    }

    private void InitializeUnitListUI()
    { 
        _unitListUI = new UnitListUI(new Position(750, 200));
        _unitListUI.ClickedOutsideEventHandler += OnUnitListUIClickedOutside;
        _unitListUI.EntryPressedEventHandler += OnOnUnitListUIEntryPressed;
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

