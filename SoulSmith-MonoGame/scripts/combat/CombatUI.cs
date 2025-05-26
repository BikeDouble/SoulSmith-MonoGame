
using Microsoft.Xna.Framework.Graphics;
using SoulSmith_MonoGame;
using System;
using SoulSmith.Drawing;
using SoulSmith.Core;

public partial class CombatUI : CanvasObject
{
	//Children
	private CanvasObject _roundCounter;

	public CombatUI(SpriteFont font) 
	{
		Initialize(font);
	}

	private void Initialize(SpriteFont font)
	{
		Position roundCounterPosition = new Position(Game1.WINDOWLENGTH/2, 30, 4, 4);

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
