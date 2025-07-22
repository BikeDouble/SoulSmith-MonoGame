using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Battle.Moves;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Drawing.Text;
using SoulSmith.Object.Canvas;
using SoulSmith.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace SoulSmith.Units;
public class UnitUIMoveMenu : CanvasObject
{
	private List<UnitUIMoveButton> _moveButtons;
	private UnitUIMoveButton _retrieveButton;

    public UnitUIMoveMenu() : base()
    {
		CreateMoveButtons();
		Hide();
		this.Scale(new Vector2(0.85f, 0.85f));
    }

	private const int TOPBUTTONX = 225;
	private const int TOPBUTTONY = -SPACEBETWEENBUTTONS;
    private const int SPACEBETWEENBUTTONS = 70;
	private const float MOVEBUTTONSCALE = 1f;
	private const int RETRIEVEBUTTONXOFFSET = -20;
	private const int RETRIEVEBUTTONYOFFSET = 58;

    private void CreateMoveButtons()
	{
		if (_moveButtons != null)
			return;

        _moveButtons = new List<UnitUIMoveButton>();

        for (int i = 0; i < 3; i++)
        {
			CreateMoveButton(i);
        }

		CreateRetrieveButton();
    }

	private void CreateMoveButton(int index)
	{
        Position buttonPosition = new Position(TOPBUTTONX, TOPBUTTONY + (index * SPACEBETWEENBUTTONS), MOVEBUTTONSCALE, MOVEBUTTONSCALE, 0, UnitUIMoveButton.ZVALUE);
		IAssetWrapper<ZonedResource> idleResource = AssetManager.Instance.GetZonedResource<ZonedResource>(UnitUIMoveButton.MOVEBUTTONIDLERESOURCEKEY);
		IAssetWrapper<ZonedResource> hoveredResource = AssetManager.Instance.GetZonedResource<ZonedResource>(UnitUIMoveButton.MOVEBUTTONIDLERESOURCEKEY);
        UnitUIMoveButton button = new UnitUIMoveButton(idleResource, hoveredResource, buttonPosition);
        _moveButtons.Add(button);
        AddChild(button);
        button.ButtonPressedEventHandler += OnMoveButtonPressed;
    }

	private void CreateRetrieveButton()
	{
        Position buttonPosition = new Position(
			TOPBUTTONX + RETRIEVEBUTTONXOFFSET, 
			TOPBUTTONY + (2 * SPACEBETWEENBUTTONS) + RETRIEVEBUTTONYOFFSET, 
			MOVEBUTTONSCALE, 
			MOVEBUTTONSCALE, 
			0, 
			UnitUIMoveButton.ZVALUE);
        IAssetWrapper<ZonedResource> idleResource = AssetManager.Instance.GetZonedResource<ZonedResource>(UnitUIMoveButton.RETRIEVEBUTTONIDLERESOURCEKEY);
        IAssetWrapper<ZonedResource> hoveredResource = AssetManager.Instance.GetZonedResource<ZonedResource>(UnitUIMoveButton.RETRIEVEBUTTONIDLERESOURCEKEY);
        UnitUIMoveButton button = new UnitUIMoveButton(idleResource, hoveredResource, buttonPosition);
		_retrieveButton = button;
        AddChild(button);
        button.ButtonPressedEventHandler += OnRetrieveButtonPressed;
    }

    // Calculates the Y offset for the retrieve button to keep the space between buttons consistent and visually appealing
    private int CalculateRetrieveButtonYOffset()
	{
        IAssetWrapper<ZonedResource> moveButtonTexture = AssetManager.Instance.GetZonedResource<ZonedResource>(UnitUIMoveButton.MOVEBUTTONIDLERESOURCEKEY);
        IAssetWrapper<ZonedResource> retrieveButtonTexture = AssetManager.Instance.GetZonedResource<ZonedResource>(UnitUIMoveButton.RETRIEVEBUTTONIDLERESOURCEKEY);
        if (moveButtonTexture == null) throw new ArgumentNullException(nameof(moveButtonTexture));
        if (retrieveButtonTexture == null) throw new ArgumentNullException(nameof(retrieveButtonTexture));

		float moveButtonHeightScale = _moveButtons[0].Position.ScaleVector.Y;
		float moveButtonHeight = moveButtonTexture.Value.GetHeightLocal(ButtonObject.CLICKZONEKEY);
		float retrieveButtonHeight = retrieveButtonTexture.Value.GetHeightLocal(ButtonObject.CLICKZONEKEY);
        int gapSize = SPACEBETWEENBUTTONS - (int)(moveButtonHeight * moveButtonHeightScale);
		int spaceBetweenMoveAndRetrieveButtons = gapSize + (int)((moveButtonHeight + retrieveButtonHeight) * moveButtonHeightScale / 2);

		moveButtonTexture.Dispose();
		retrieveButtonTexture.Dispose();

		return spaceBetweenMoveAndRetrieveButtons;
    }

	// Calculates the X offset for the retrieve button to keep the buttons left aligned
	private int CalculateRetrieveButtonXOffset()
	{
        IAssetWrapper<ZonedResource> moveButtonTexture = AssetManager.Instance.GetZonedResource<ZonedResource>(UnitUIMoveButton.MOVEBUTTONIDLERESOURCEKEY);
        IAssetWrapper<ZonedResource> retrieveButtonTexture = AssetManager.Instance.GetZonedResource<ZonedResource>(UnitUIMoveButton.RETRIEVEBUTTONIDLERESOURCEKEY);
        if (moveButtonTexture == null) throw new ArgumentNullException(nameof(moveButtonTexture));
        if (retrieveButtonTexture == null) throw new ArgumentNullException(nameof(retrieveButtonTexture));

        float moveButtonWidthScale = _moveButtons[0].Position.ScaleVector.X;
        float moveButtonWidth = moveButtonTexture.Value.GetWidthLocal(ButtonObject.CLICKZONEKEY);
        float retrieveButtonWidth = retrieveButtonTexture.Value.GetWidthLocal(ButtonObject.CLICKZONEKEY);
		int leftMovement = (int)((retrieveButtonWidth - moveButtonWidth) * moveButtonWidthScale / 2);

        moveButtonTexture.Dispose();
        retrieveButtonTexture.Dispose();

        return leftMovement;
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

		_retrieveButton.UpdateButtonAsRetrieve();
	}

	public event EventHandler<MoveButtonPressedEventArgs> MoveButtonPressedEventHandler;

	//Listens to move buttons
	private void OnMoveButtonPressed(object sender, ButtonPressedEventArgs args)
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

    public event EventHandler<RetrieveButtonPressedEventArgs> RetrieveButtonPressedEventHandler;

    private void OnRetrieveButtonPressed(object sender, ButtonPressedEventArgs args)
	{
        RetrieveButtonPressedEventArgs e = new RetrieveButtonPressedEventArgs();

		RetrieveButtonPressedEventHandler?.Invoke(this, e);
    }
}

public class MoveButtonPressedEventArgs : EventArgs
{
    public Move Move;
    public Unit Sender;
}

public class RetrieveButtonPressedEventArgs : EventArgs
{
	public Unit Retrievee;
}