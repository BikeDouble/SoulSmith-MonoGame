using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Battle.Moves;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.Object.Canvas;
using SoulSmith.Drawing.Text;
using SoulSmith.Emotion;

namespace SoulSmith.Units;
public class UnitUIMoveButton : ButtonObject
{
    public const string MOVEBUTTONIDLERESOURCEKEY = "ZonedResources/UI/Units/MoveButton";
    public const string RETRIEVEBUTTONIDLERESOURCEKEY = "ZonedResources/UI/Units/RetrieveButton";
    public const string LABELFONTKEY = "Fonts/Raleway/Medium";
	public const int ZVALUE = (int)ZLayer.UnitMoveButton;
    private const float IDLEDIMNESSMULT = 0.8f;
	private const float HOVERSIZEMOD = 1.1f;
	private const float WIDTHSCALE = 0.35f;
	private const float HEIGHTSCALE = 0.35f;
	private const float LABELWIDTHSCALE = 1.3f;
	private const float LABELHEIGHTSCALE = 1.3f;
	public const int LABELBRIGHTNESS = 80;
    private static Position _hoverTransformation = new Position(0, 0, HOVERSIZEMOD, HOVERSIZEMOD);
    private static Position _unhoverTransformation = new Position(0, 0, 1 / HOVERSIZEMOD, 1 / HOVERSIZEMOD);

	// Children
    private CanvasObject _label;

	private Move _move = null;
	private Color _idleColor = Color.Gray;
	private Color _hoverColor = Color.Gray;
	private bool _isRetrieveButton = false;

	public UnitUIMoveButton(
        IAssetWrapper<ZonedResource> idleResource,
        IAssetWrapper<ZonedResource> hoveredResource,
        Position position = null) : base(
            idleResource,
			hoveredResource,
			position) 
	{
		IAssetWrapper<IDrawableResource> textResource = DrawHelpers.GetDrawableResource(LABELFONTKEY, "simpletextresource");
        _label = new CanvasObject(new Position(0, 0, LABELWIDTHSCALE, LABELHEIGHTSCALE, 0, ZVALUE + 1), textResource);
		_label.SetColor(new Color(LABELBRIGHTNESS, LABELBRIGHTNESS, LABELBRIGHTNESS, 255));
        AddChild(_label);
		_label.UpdateResourceState("0");
		this.Scale(new Vector2(WIDTHSCALE, HEIGHTSCALE));
    }

	public void UpdateButtonWithMove(Move move)
	{
		_isRetrieveButton = false;
		_move = move;
		SetLabelText(move.FriendlyName);
		SetEmotionColor(move.EmotionTag);
		this.Show();
		this.SetColor(_idleColor);
    }

	public void UpdateButtonAsRetrieve()
	{
		_isRetrieveButton = true;
		_move = null;
		SetLabelText("Retrieve");
		SetEmotionColor(EmotionTag.EmotionTag.Typeless);
		this.Show();
		this.SetColor(_idleColor);
	}

	private void SetEmotionColor(EmotionTag.EmotionTag emotionTag)
	{
		Emotion.Emotion emotion = Emotion.Emotion.GetEmotion(emotionTag);

		if (emotion == null) _hoverColor = Color.Gray;
        else _hoverColor = emotion.Color;

		_idleColor = new Color(
			(int)(_hoverColor.R * IDLEDIMNESSMULT), 
			(int)(_hoverColor.G * IDLEDIMNESSMULT), 
			(int)(_hoverColor.B * IDLEDIMNESSMULT),
			_hoverColor.A) ;

		this.SetColor(_idleColor);
    }

	public void UpdateButtonAsEmptySlot()
	{
		_isRetrieveButton = false;
		_move = null;
        SetLabelText("Empty");
		_hoverColor = Color.DimGray;
        _idleColor = new Color(
            (int)(_hoverColor.R * IDLEDIMNESSMULT),
            (int)(_hoverColor.G * IDLEDIMNESSMULT),
            (int)(_hoverColor.B * IDLEDIMNESSMULT),
            _hoverColor.A);
        this.SetColor(_idleColor);
		this.Hide();
    }

    private void SetLabelText(string text)
	{
		_label.UpdateResourceState(text);
	}

	public override void OnMouseEnter()
	{
		base.OnMouseEnter();

		if ((_move != null) || (_isRetrieveButton))
		{
			Transform(_hoverTransformation);
			this.SetColor(_hoverColor);
		}
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();

		if ((_move != null) || (_isRetrieveButton))
        {
			Transform(_unhoverTransformation);
			this.SetColor(_idleColor);
		}
    }

    public Move Move { get { return _move; } private set { _move = value; } }
}