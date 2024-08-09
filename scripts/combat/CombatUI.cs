
using Microsoft.Xna.Framework.Graphics;
using SoulSmith_MonoGame;
using System;

public partial class CombatUI : CanvasItem
{
	//Children
	private CanvasItem _roundCounter;

	public CombatUI(SpriteFont font) 
	{
		Initialize(font);
	}

	private void Initialize(SpriteFont font)
	{
		Position roundCounterPosition = new Position(Game1.WINDOWLENGTH/2, 30, 4, 4);

		_roundCounter = new CanvasItem(font, null, roundCounterPosition);
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
