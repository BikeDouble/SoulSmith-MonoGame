using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Battle.Moves;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.Object.Canvas;
using SoulSmith.Drawing.Text;
using System.Globalization;

namespace SoulSmith.Units;
public class UnitUIMoveMenu : CanvasObject
{
	private List<UnitUIMoveButton> _moveButtons;

    public UnitUIMoveMenu(IAssetWrapper<ZonedResource> moveButtonResource) : base()
    {
		CreateMoveButtons(moveButtonResource);
		Hide();
		this.Scale(new Vector2(0.9f, 0.9f));
    }

	private const int TOPBUTTONX = 225;
	private const int TOPBUTTONY = -SPACEBETWEENBUTTONS;
    private const int SPACEBETWEENBUTTONS = 70;
	private const float MOVEBUTTONSCALE = 1f;

    private void CreateMoveButtons(IAssetWrapper<ZonedResource> moveButton)
	{
		if (_moveButtons != null)
			return;

        _moveButtons = new List<UnitUIMoveButton>();

		CreateMoveButton(0, moveButton);
        for (int i = 1; i < 3; i++)
        {
			CreateMoveButton(i, moveButton);
        }
    }

	private void CreateMoveButton(int index, IAssetWrapper<ZonedResource> moveButton)
	{
        Position buttonPosition = new Position(TOPBUTTONX, TOPBUTTONY + (index * SPACEBETWEENBUTTONS), MOVEBUTTONSCALE, MOVEBUTTONSCALE, 0, UnitUIMoveButton.ZVALUE);
        UnitUIMoveButton button = new UnitUIMoveButton(moveButton, buttonPosition);
        _moveButtons.Add(button);
        AddChild(button);
        button.ButtonPressedEventHandler += OnMoveButtonPressed;
    }

    public void UpdateMoveMenu(ReadOnlyCollection<Move> moves)
	{
		UpdateMoveButtons(moves);
	}
	
	private void UpdateMoveButtons(ReadOnlyCollection<Move> moves)
	{
		int moveCount = moves.Count;

		for (int i = 0; i < 3; i++)
		{
			if (i < moveCount)
			{
				_moveButtons[i].UpdateButtonWithMove(moves[i]);
			}
			else
			{
				_moveButtons[i].UpdateButtonAsEmptySlot();
			}
		}
	}

	public event EventHandler<MoveButtonPressedEventArgs> MoveButtonPressedEventHandler;

	//Listens to move buttons
	public void OnMoveButtonPressed(object sender, ButtonPressedEventArgs args)
	{
		UnitUIMoveButton button = sender as UnitUIMoveButton;

		if (button is null)
			return;

		Move move = button.Move;

		if (move == null)
			return;

		MoveButtonPressedEventArgs e = new();

		e.Move = move;

		MoveButtonPressedEventHandler(this, e);
	}
}

public class MoveButtonPressedEventArgs : EventArgs
{
    public Move Move;
    public Unit Sender;
}