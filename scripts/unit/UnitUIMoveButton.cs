using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmithMoves;

public partial class UnitUIMoveButton : Button
{
	private const float IDLEDIMNESS = 0.6f;

	private CanvasItem _label;

	private Move _move = null;
	private Color _idleColor = Color.Gray;
	private Color _hoverColor = Color.Gray;

	public UnitUIMoveButton() : base()
	{
		_label = new CanvasItem();
		AddChild( _label );
	}

	public UnitUIMoveButton(SpriteFont font, TrackedResource<ColoredPolygon> resource, Position position = null) : base(resource, position) 
	{
        _label = new CanvasItem(font, "O");
        AddChild(_label);
    }

	public void UpdateButtonWithMove(Move move)
	{		
		_move = move;
		SetLabelText(move.FriendlyName);
		SetEmotionColor(move.Emotion.Color);
		Resource.UpdateColor(_idleColor);
	}

	private void SetEmotionColor(Color color)
	{
		_hoverColor = color;
		_idleColor = new Color((int)(color.R * IDLEDIMNESS), (int)(color.G * IDLEDIMNESS), (int)(color.B * IDLEDIMNESS)) ;
	}

	public void UpdateButtonAsEmptySlot()
	{
		SetLabelText("Empty");
		_hoverColor = Color.Gray;
		_idleColor = Color.Gray;
	}

    public override void OnMouseEnter()
    {
		Resource.UpdateColor(_hoverColor);
        base.OnMouseEnter();
    }

    public override void OnMouseExit()
    {
		Resource.UpdateColor(_idleColor);
        base.OnMouseExit();
    }

    private void SetLabelText(string text)
	{
		_label.UpdateText(text);
	}

    public Move Move { get { return _move; } private set { _move = value; } }
}