using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using SoulSmith.Battle.Move;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Units;
public class UnitUIMoveMenu : CanvasObject
{
	private List<UnitUIMoveButton> _moveButtons;

    public UnitUIMoveMenu(SpriteFont font, IReadOnlyTrackedAsset<ZonedResource> moveButtonResource = null) : base()
    {
		CreateMoveButtons(font, moveButtonResource);
		Hide();
    }

	private const int TOPBUTTONX = 200;
	private const int TOPBUTTONY = -SPACEBETWEENBUTTONS;
    private const int SPACEBETWEENBUTTONS = 70;
	private const float MOVEBUTTONSCALE = 1f;

    private void CreateMoveButtons(SpriteFont font, IReadOnlyTrackedAsset<ZonedResource> resource)
	{
		if (_moveButtons != null)
			return;

        _moveButtons = new List<UnitUIMoveButton>();

		CreateMoveButton(0, font, resource);
        for (int i = 1; i < 3; i++)
        {
			CreateMoveButton(i, font, new TrackedAsset<ZonedResource>(resource));
        }
    }

	private void CreateMoveButton(int index, SpriteFont font, IReadOnlyTrackedAsset<ZonedResource> resource)
	{
        Position buttonPosition = new Position(TOPBUTTONX, TOPBUTTONY + (index * SPACEBETWEENBUTTONS), MOVEBUTTONSCALE, MOVEBUTTONSCALE);
        UnitUIMoveButton button = new UnitUIMoveButton(font, resource, buttonPosition);
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