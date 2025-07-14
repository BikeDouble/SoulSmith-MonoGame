
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Object.Canvas;
using System;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.Drawing.Text;
using SoulSmith.Units;

namespace SoulSmith.Combat;
public class CombatUI : CanvasObject
{
	public const string FONTKEY = "Fonts/Raleway/Medium";

	//Children
	private CanvasObject _roundCounter;

	public CombatUI() 
	{
		Initialize();
	}

	private void Initialize()
	{
        Position roundCounterPosition = new Position(1600/2, 30, 4, 4);

		IAssetWrapper<IDrawableResource> roundCounterText = DrawHelpers.GetDrawableResource(FONTKEY, "simpletextresource");
		_roundCounter = new CanvasObject(roundCounterPosition, roundCounterText);
		AddChild(_roundCounter);
	}

	public void Update(int roundNumber)
	{
		UpdateRoundCounter(roundNumber);
	}

	private void UpdateRoundCounter(int roundNumber)
	{
		_roundCounter.UpdateResourceState(roundNumber.ToString());
	}
}
