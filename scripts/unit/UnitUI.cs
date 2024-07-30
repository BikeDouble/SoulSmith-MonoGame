using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using SoulSmithMoves;

public partial class UnitUI : CanvasItem
{ 
	//Children
	private UnitUIMoveMenu _moveMenu;
	private UnitUIHealthBar _healthBar;
	private Button _targetButton; //TODO

	public UnitUI(SpriteFont font, DrawableResource_Polygon moveButton, DrawableResource_Polygon targetButton)
	{
        _moveMenu = new UnitUIMoveMenu(font, moveButton);
        _moveMenu.MoveButtonPressedEventHandler += OnMoveButtonPressed;
        AddChild(_moveMenu);

		Position healthBarPosition = new Position(0, 120);
        _healthBar = new UnitUIHealthBar(font, healthBarPosition);
        AddChild(_healthBar);
        _targetButton = new Button(new DrawableResource_Polygon(targetButton), new DrawableResource_Polygon(targetButton));
        _targetButton.ButtonPressedEventHandler += OnTargetButtonPressed;
        AddChild(_targetButton);

        Initialize();
    }

	private void Initialize()
	{
		
	}
	
	public void Update(UnitStats stats)
	{
		UpdateHealthBar(stats);
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
	
	private void UpdateHealthBar(UnitStats stats)
	{
		_healthBar.Update(stats);
	}
	
}

public class TargetButtonPressedEventArgs : EventArgs
{
	public Unit Target;
}

