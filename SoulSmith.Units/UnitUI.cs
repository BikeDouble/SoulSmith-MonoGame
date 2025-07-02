using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Battle.Modifier;
using SoulSmith.Battle.Move;
using SoulSmith.Battle;
using SoulSmith.Object.Canvas;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.UnitStats;

namespace SoulSmith.Units;
public class UnitUI : CanvasObject
{
	public const float TARGETBUTTONWIDTHSCALE = 0.35f;
	public const float TARGETBUTTONHEIGHTSCALE = 0.35f;

	//Children
	private UnitUIMoveMenu _moveMenu;
	private UnitUIHealthBar _healthBar;
	private UnitUIModifierDisplay _modifierDisplay;
	private UnitUITimeOnBoardDisplay _timeOnBoardDisplay = null;
	private ButtonObject _targetButton;

	private readonly SpriteFont _font;

	public UnitUI() :
		this(null,//MasterAssetLoader.GetFont(GameManager.UIFONTNAME), TODO fix fonts
			AssetManager.Instance.GetZonedTexture2D<ZonedResource>("ZonedTextures/UI/Units/MoveButton"),
            AssetManager.Instance.GetZonedTexture2D<ZonedResource>("ZonedTextures/UI/Units/TargetButtonIdle"),
            AssetManager.Instance.GetZonedTexture2D<ZonedResource>("ZonedTextures/UI/Units/TargetButtonHovered")) 
	{ }

	public UnitUI(SpriteFont font, IReadOnlyTrackedAsset<ZonedResource> moveButton, IReadOnlyTrackedAsset<ZonedResource> targetButtonIdle, IReadOnlyTrackedAsset<ZonedResource> targetButtonHovered)
		: base(new Position(0, 0, 1, 1, 0, 5))
	{
		if (moveButton == null) throw new ArgumentNullException(nameof(moveButton));
		if (targetButtonIdle == null) throw new ArgumentNullException(nameof(targetButtonIdle));
		if (targetButtonHovered == null) throw new ArgumentNullException(nameof(targetButtonHovered));

		_font = font;

        _moveMenu = new UnitUIMoveMenu(_font, moveButton);
        _moveMenu.MoveButtonPressedEventHandler += OnMoveButtonPressed;
        AddChild(_moveMenu);

		Position healthBarPosition = new Position(0, 120, 1, 1, 0, 0);
        _healthBar = new UnitUIHealthBar(_font, healthBarPosition);
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

