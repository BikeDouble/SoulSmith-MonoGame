using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Battle.Moves;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.Object.Canvas;
using SoulSmith.Drawing.Text;
using SoulSmith.Drawing.Zoned;

namespace SoulSmith.Units;
public class UnitUIMoveButton : ButtonObject_GrowOnHover
{
    public const string MOVEBUTTONIDLERESOURCEKEY = "ZonedResources/UI/Units/Moves/MoveButton";
    public const string RETRIEVEBUTTONIDLERESOURCEKEY = "ZonedResources/UI/Units/Moves/RetrieveButton";
    public const string LABELFONTKEY = "Fonts/Raleway/Medium";
    public const float IDLEDIMNESSMULT = 0.8f;
	public const float HOVERSIZEMOD = 1.1f;
	public const float WIDTHSCALE = 0.35f;
	public const float HEIGHTSCALE = 0.35f;
	public const float LABELWIDTHSCALE = 1.3f;
	public const float LABELHEIGHTSCALE = 1.3f;
	public const int LABELBRIGHTNESS = 80;
	public const int MOVEBUTTONDISTANCEFROMEDFEATCENTER = 61;
	public const int DESCRIPTIONDISPLAYXOFFSET = 512 - (2 * MOVEBUTTONDISTANCEFROMEDFEATCENTER);
	public const float TIMEHOVEREDFORDESCRIPTIONDISPLAY = 0.5f;

    // Children
    private CanvasObject _label;
	private UnitUIMoveDescriptionDisplay _descriptionDisplay;

	private Move _move = null;
	private Color _idleColor = Color.Gray;
	private Color _hoverColor = Color.Gray;
	private bool _isRetrieveButton = false;

	public UnitUIMoveButton(
        ZonedDrawableResourceInstance idleResource,
        ZonedDrawableResourceInstance hoveredResource,
        Position position = null) : base(
            idleResource,
			hoveredResource,
			position,
			new Vector2(HOVERSIZEMOD, HOVERSIZEMOD)) 
	{
		InitializeLabel();
		InitializeDescriptionDisplay();
        this.Scale(new Vector2(WIDTHSCALE, HEIGHTSCALE));
    }

	private void InitializeDescriptionDisplay()
	{
		_descriptionDisplay = new UnitUIMoveDescriptionDisplay(MOVEBUTTONIDLERESOURCEKEY, new Position(new Vector2(DESCRIPTIONDISPLAYXOFFSET, 0), new Vector2(1, 1), 0, -1));
		_descriptionDisplay.SetColor(new Color(IDLEDIMNESSMULT, IDLEDIMNESSMULT, IDLEDIMNESSMULT));
		_descriptionDisplay.Hide();
        AddChild(_descriptionDisplay);
    }

    private void InitializeLabel()
	{
        IDrawableResource textResource = DrawHelpers.GetDrawableResourceInstance(LABELFONTKEY);
		_label = new CanvasObject(new Position(0, 0, LABELWIDTHSCALE, LABELHEIGHTSCALE, 0, 1), textResource);
		_label.SetColor(new Color(LABELBRIGHTNESS, LABELBRIGHTNESS, LABELBRIGHTNESS, 255));
		AddChild(_label);
		_label.UpdateResourceState("0");
    }

    public void UpdateButtonWithMove(Move move)
	{
		this.EnableGrow();
		_isRetrieveButton = false;
		_move = move;
		SetLabelText(move.FriendlyName);
		_descriptionDisplay.UpdateDescription(move.Description);
        SetEmotionColor(move.EmotionTag);
		this.Show();
		this.SetColor(_idleColor);
    }

	public void UpdateButtonAsRetrieve()
	{
		this.EnableGrow();
		_isRetrieveButton = true;
		_move = null;
		SetLabelText("Retrieve");
		SetEmotionColor(EmotionTags.EmotionTag.Typeless);
		this.Show();
		this.SetColor(_idleColor);
	}

	private void SetEmotionColor(EmotionTags.EmotionTag emotionTag)
	{
        Battle.Emotions.Emotion emotion = Battle.Emotions.Emotion.GetEmotion(emotionTag);

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
		this.DisableGrow();
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
			this.SetColor(_hoverColor);
		}
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();

		if ((_move != null) || (_isRetrieveButton))
        {
			this.SetColor(_idleColor);
		}

		_descriptionDisplay.Hide();
    }

    public override void Process(double delta)
    {
        base.Process(delta);

		if (!_isRetrieveButton)
		{
			if (TimeHovered > TIMEHOVEREDFORDESCRIPTIONDISPLAY) _descriptionDisplay.Show();
		}
    }

    public Move Move { get { return _move; } private set { _move = value; } }
}