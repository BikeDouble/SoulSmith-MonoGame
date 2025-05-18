using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using SoulSmithModifiers;
using SoulSmithMoves;
using SoulSmithObjects;
using SoulSmithUnitUI;
using SoulSmith.Drawing;

public partial class UnitUI : CanvasItem
{ 
	//Children
	private UnitUIMoveMenu _moveMenu;
	private UnitUIHealthBar _healthBar;
	private UnitUIModifierDisplay _modifierDisplay;
	private UnitUITimeOnBoardDisplay _timeOnBoardDisplay = null;
	private Button _targetButton;
	private readonly SpriteFont _font;


	public UnitUI() :
		this(AssetLoader.GetFont(GameManager.UIFONTNAME),
			new DrawableResource_Polygon(AssetLoader.GetPolygon("MoveButton").Resource), //TODO fix TrackedAsset
            new DrawableResource_Polygon(AssetLoader.GetPolygon("TargetButtonIdle").Resource),
            new DrawableResource_Polygon(AssetLoader.GetPolygon("TargetButtonHovered").Resource))
	{ }

	public UnitUI(SpriteFont font, DrawableResource_Polygon moveButton, DrawableResource_Polygon targetButton, DrawableResource_Polygon targetButtonHovered)
		: base(new CanvasPosition(0, 0, 1, 1, 0, 5))
	{
		_font = font;

        _moveMenu = new UnitUIMoveMenu(_font, moveButton);
        _moveMenu.MoveButtonPressedEventHandler += OnMoveButtonPressed;
        AddChild(_moveMenu);

		CanvasPosition healthBarPosition = new CanvasPosition(0, 120, 1, 1, 0, 0);
        _healthBar = new UnitUIHealthBar(_font, healthBarPosition);
        AddChild(_healthBar);

        _targetButton = new Button(new DrawableResource_Polygon(targetButton), new DrawableResource_Polygon(targetButtonHovered));
		_targetButton.Hide();
        _targetButton.ButtonPressedEventHandler += OnTargetButtonPressed;
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
			if (_timeOnBoardDisplay == null)
			{
				_timeOnBoardDisplay = new UnitUITimeOnBoardDisplay(_font);
				AddChild(_timeOnBoardDisplay);
			}

			_timeOnBoardDisplay.UpdateText(timeOnBoard.ToString());
		}
    }

	//
	// Modifier related functions
	//

	public void OnModifierAdded(Modifier modifier)
	{
		_modifierDisplay.OnModifierAdded(modifier);
	}

	public void OnModifierRemoved(Modifier modifier)
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
		MoveButtonPressedEventHandler(this, e);
	}

	public event EventHandler<TargetButtonPressedEventArgs> TargetButtonPressedEventHandler;
	
	private void OnTargetButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		TargetButtonPressedEventArgs args = new();

		TargetButtonPressedEventHandler(this, args);
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

