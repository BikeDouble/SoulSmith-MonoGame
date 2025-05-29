
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Object.Canvas;
using System;
using SoulSmith.Drawing;
using SoulSmith.Core;

namespace SoulSmith.Combat;
public class CombatUI : CanvasObject
{
	//Children
	private CanvasObject _roundCounter;

	public CombatUI(SpriteFont font) 
	{
		Initialize(font);
	}

	private void Initialize(SpriteFont font)
	{
		Position roundCounterPosition = new Position(1600/2, 30, 4, 4);

		_roundCounter = new CanvasObject(font, null, roundCounterPosition);
		AddChild(_roundCounter);
	}

	public void Update(int roundNumber)
	{
		UpdateRoundCounter(roundNumber);
	}

	private void UpdateRoundCounter(int roundNumber)
	{
		_roundCounter.UpdateText(roundNumber.ToString());
	}
}
