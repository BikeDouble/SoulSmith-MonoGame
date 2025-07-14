using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Battle.Move;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.Object.Canvas;
using SoulSmith.Drawing.Text;

namespace SoulSmith.Units;
public class UnitUIMoveButton : ButtonObject
{
    public const string LABELFONTKEY = "Fonts/Raleway/Medium";
	public const int ZVALUE = 10;
    private const float IDLEDIMNESSMULT = 0.8f;
	private const float HOVERSIZEMOD = 1.1f;
	private const float WIDTHSCALE = 0.35f;
	private const float HEIGHTSCALE = 0.35f;
	private const float LABELWIDTHSCALE = 1.5f;
	private const float LABELHEIGHTSCALE = 1.5f;

	private CanvasObject _label;

	private Move _move = null;
	private Color _idleColor = Color.Gray;
	private Color _hoverColor = Color.Gray;
	private static Position _hoverTransformation = new Position(0, 0, HOVERSIZEMOD, HOVERSIZEMOD);
	private static Position _unhoverTransformation = new Position(0, 0, 1 / HOVERSIZEMOD, 1 / HOVERSIZEMOD);

	public UnitUIMoveButton(
		IAssetWrapper<ZonedResource> resource,
		Position position = null) : base(
			resource, 
			resource,  
			position) 
	{
		IAssetWrapper<IDrawableResource> textResource = DrawHelpers.GetDrawableResource(LABELFONTKEY, "simpletextresource");
        _label = new CanvasObject(new Position(0, 0, LABELWIDTHSCALE, LABELHEIGHTSCALE, 0, ZVALUE + 1), textResource);
		_label.ChangeColorAdditive(-255, -255, -255, 0);
        AddChild(_label);
		_label.UpdateResourceState("0");
		this.Scale(new Vector2(WIDTHSCALE, HEIGHTSCALE));
    }

	public void UpdateButtonWithMove(Move move)
	{		
		_move = move;
		SetLabelText(move.FriendlyName);
		SetEmotionColor(Color.White);//move.EmotionTag.Color); //TODO fix coloring
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
		_label.UpdateResourceState(text);
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