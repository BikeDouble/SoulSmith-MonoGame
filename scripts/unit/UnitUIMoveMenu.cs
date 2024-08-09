using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using SoulSmithMoves;

public partial class UnitUIMoveMenu : CanvasItem
{
	private List<UnitUIMoveButton> _moveButtons;

    public UnitUIMoveMenu(SpriteFont font, DrawableResource_Polygon moveButtonResource = null) : base()
    {
		CreateMoveButtons(font, moveButtonResource);
    }

	private const int TOPBUTTONX = 200;
	private const int TOPBUTTONY = -SPACEBETWEENBUTTONS;
    private const int SPACEBETWEENBUTTONS = 70;
	private const float MOVEBUTTONSCALE = 1f;

    private void CreateMoveButtons(SpriteFont font, DrawableResource_Polygon resource = null)
	{
		if (_moveButtons != null)
			return;

        _moveButtons = new List<UnitUIMoveButton>();

        for (int i = 0; i < 3; i++)
        {
			Position buttonPosition = new Position(TOPBUTTONX, TOPBUTTONY + (i * SPACEBETWEENBUTTONS), MOVEBUTTONSCALE, MOVEBUTTONSCALE);
            UnitUIMoveButton button = new UnitUIMoveButton(font, (DrawableResource_Polygon)resource?.DeepClone(), buttonPosition);
			_moveButtons.Add(button);
            AddChild(button);
            button.ButtonPressedEventHandler += OnMoveButtonPressed;
        }
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