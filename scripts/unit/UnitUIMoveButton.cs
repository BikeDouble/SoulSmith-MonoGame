using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmithMoves;

public partial class UnitUIMoveButton : Button
{
	private const float IDLEDIMNESSMULT = 0.8f;
	private const float HOVERSIZEMOD = 1.1f;

	private CanvasItem _label;

	private Move _move = null;
	private Color _idleColor = Color.Gray;
	private Color _hoverColor = Color.Gray;
	private static Position _hoverTransformation = new Position(0, 0, HOVERSIZEMOD, HOVERSIZEMOD);
	private static Position _unhoverTransformation = new Position(0, 0, 1 / HOVERSIZEMOD, 1 / HOVERSIZEMOD);

	public UnitUIMoveButton() : base()
	{
		_label = new CanvasItem();
		AddChild( _label );
	}

	public UnitUIMoveButton(
		SpriteFont font, 
		DrawableResource_Polygon resource,
		Position position = null) : base(
			resource, 
			(DrawableResource)resource.DeepClone(), 
			null, 
			position) 
	{
        _label = new CanvasItem(font, "O");
        AddChild(_label);
    }

	public void UpdateButtonWithMove(Move move)
	{		
		_move = move;
		SetLabelText(move.FriendlyName);
		SetEmotionColor(move.Emotion.Color);
		IdleResource?.UpdateColor(_idleColor);
		HoveredResource?.UpdateColor(_hoverColor);
	}

	private void SetEmotionColor(Color color)
	{
		_hoverColor = color;
		_idleColor = new Color(
			(int)(color.R * IDLEDIMNESSMULT), 
			(int)(color.G * IDLEDIMNESSMULT), 
			(int)(color.B * IDLEDIMNESSMULT),
			color.A) ;
	}

	public void UpdateButtonAsEmptySlot()
	{
		SetLabelText("Empty");
		_hoverColor = Color.Gray;
		_idleColor = Color.Gray;
	}

    private void SetLabelText(string text)
	{
		_label.UpdateText(text);
	}

	public override void OnMouseEnter()
	{
		base.OnMouseEnter();

		Transform(_hoverTransformation);
	}

    public override void OnMouseExit()
    {
        base.OnMouseExit();

		Transform(_unhoverTransformation);
    }

    public Move Move { get { return _move; } private set { _move = value; } }
}