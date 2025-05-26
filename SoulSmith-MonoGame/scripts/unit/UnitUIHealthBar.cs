
using System;
using Microsoft.Xna.Framework.Graphics;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmith.Drawing;
using SoulSmith.Core;

public partial class UnitUIHealthBar : CanvasObject
{
	//Children
	private CanvasObject _backboard;
	private CanvasObject _background;
	private CanvasObject _healthbar;
	private CanvasObject _healthText = null;

	public UnitUIHealthBar(SpriteFont font, Position position = null) : base(position)
	{
		_healthText = new CanvasObject(font);
		AddChild(_healthText);
	}

	public void Update(IReadOnlyUnitStats stats)
	{
		int curHealth = stats.GetModStat(StatType.CurHealth);
		int maxHealth = stats.GetModStat(StatType.MaxHealth);
		string newText = "HP: " + curHealth + " / " + maxHealth;
		_healthText.UpdateText(newText);
	}
}
