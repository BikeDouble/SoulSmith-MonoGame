using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Moves;
using SoulSmith.Battle;
using SoulSmith.Object.Canvas;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.UnitStats;
using SoulSmith.Drawing.Text;
using SoulSmith.Drawing.Zoned;

namespace SoulSmith.Units;
public class UnitUI : CanvasObject
{
	public const float TARGETBUTTONWIDTHSCALE = 0.35f;
	public const float TARGETBUTTONHEIGHTSCALE = 0.35f;
	public const string TARGETBUTTONIDLERESOURCEKEY = "ZonedResources/UI/Units/TargetButtonIdle";
    public const string TARGETBUTTONHOVEREDRESOURCEKEY = "ZonedResources/UI/Units/TargetButtonHovered";

    //Children
    private UnitUIMoveMenu _moveMenu;
	private UnitUIHealthBar _healthBar;
	private UnitUIModifierDisplay _modifierDisplay;
	private UnitUITimeOnBoardDisplay _timeOnBoardDisplay = null;
	private ButtonObject _targetButton;

	public UnitUI() :
		this(AssetManager.Instance.GetZonedResource<ZonedResource>(TARGETBUTTONIDLERESOURCEKEY),
            AssetManager.Instance.GetZonedResource<ZonedResource>(TARGETBUTTONHOVEREDRESOURCEKEY)) 
	{ }

	public UnitUI(IAssetWrapper<ZonedResource> targetButtonIdle, IAssetWrapper<ZonedResource> targetButtonHovered)
		: base(new Position(0, 0, 1, 1, 0, 5))
	{
		if (targetButtonIdle == null) throw new ArgumentNullException(nameof(targetButtonIdle));
		if (targetButtonHovered == null) throw new ArgumentNullException(nameof(targetButtonHovered));

		_moveMenu = new UnitUIMoveMenu();
        _moveMenu.MoveButtonPressedEventHandler += OnMoveButtonPressed;
		_moveMenu.RetrieveButtonPressedEventHandler += OnRetrieveButtonPressed;
        AddChild(_moveMenu);

		Position healthBarPosition = new Position(0, 120, 1, 1, 0, 0);
        _healthBar = new UnitUIHealthBar(healthBarPosition);
        AddChild(_healthBar);

        _targetButton = new ButtonObject(targetButtonIdle, targetButtonHovered);
		_targetButton.Hide();
        _targetButton.ButtonPressedEventHandler += OnTargetButtonPressed;
		_targetButton.Scale(new Microsoft.Xna.Framework.Vector2(TARGETBUTTONWIDTHSCALE, TARGETBUTTONHEIGHTSCALE));
        AddChild(_targetButton);

		_modifierDisplay = new UnitUIModifierDisplay();
		AddChild(_modifierDisplay);

        Initialize();
    }

	private void Initialize()
	{
		
	}
	
	public void Update(IReadOnlyUnitStats stats)
	{
		UpdateHealthBar(stats);
		UpdateRoundsOnBoardCounter(stats);
	}

	private void UpdateRoundsOnBoardCounter(IReadOnlyUnitStats stats)
	{
		int timeOnBoard = stats.TimeOnBoard;

		if (timeOnBoard > -1)
		{
			/*if (_timeOnBoardDisplay == null) //TODO move declaration
			{
				_timeOnBoardDisplay = new UnitUITimeOnBoardDisplay(_font);
				AddChild(_timeOnBoardDisplay);
			}

			_timeOnBoardDisplay.UpdateText(timeOnBoard.ToString());*/
		}
    }

	//
	// Modifier related functions
	//

	public void OnModifierAdded(IModifier modifier)
	{
		_modifierDisplay.OnModifierAdded(modifier);
	}

	public void OnModifierRemoved(IModifier modifier)
	{
		_modifierDisplay.OnModifierRemoved(modifier);
	}

	//
	// Move related functions
	//
	
	public void UpdateMoveMenu(ReadOnlyCollection<Move> moveSet)
	{
		_moveMenu.UpdateMoveMenu(moveSet);
	}
	
	// 
	// Move selection related functions
	//
	
	public void ShowMoveSelect()
	{
		_moveMenu.Show();
	}
	
	public void HideMoveSelect()
	{
		_moveMenu.Hide();
	}
	
	public void ShowTargetSelect()
	{
		_targetButton.Show();
	}
	
	public void HideTargetSelect()
	{
		_targetButton.Hide();
	}

	public event EventHandler<MoveButtonPressedEventArgs> MoveButtonPressedEventHandler;

	//Listens to moveMenu
	public void OnMoveButtonPressed(object sender, MoveButtonPressedEventArgs e)
	{
		MoveButtonPressedEventHandler?.Invoke(this, e);
	}

	public event EventHandler<RetrieveButtonPressedEventArgs> RetrieveButtonPressedEventHandler;

    //Listens to retrieve button
    public void OnRetrieveButtonPressed(object sender, RetrieveButtonPressedEventArgs e)
	{
		RetrieveButtonPressedEventHandler?.Invoke(this, e);
	}

    public event EventHandler<TargetButtonPressedEventArgs> TargetButtonPressedEventHandler;
	
	private void OnTargetButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		TargetButtonPressedEventArgs args = new();

		TargetButtonPressedEventHandler?.Invoke(this, args);
	}
	
	//
	// Healthbar related functions
	//
	
	private void UpdateHealthBar(IReadOnlyUnitStats stats)
	{
		_healthBar.Update(stats);
	}

}

public class TargetButtonPressedEventArgs : EventArgs
{
	public Unit Target;
}

