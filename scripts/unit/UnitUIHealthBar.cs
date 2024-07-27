
using System;
using Microsoft.Xna.Framework.Graphics;
using SoulSmithMoves;
using SoulSmithStats;

public partial class UnitUIHealthBar : CanvasItem
{
	private CanvasItem _healthText = null;

	public UnitUIHealthBar(SpriteFont font, Position position = null) : base(position)
	{
		_healthText = new CanvasItem(font);
		AddChild(_healthText);
	}

	public void Update(UnitStats stats)
	{
		int curHealth = stats.GetModStat(StatType.CurHealth);
		int maxHealth = stats.GetModStat(StatType.MaxHealth);
		string newText = "HP: " + curHealth + " / " + maxHealth;
		_healthText.UpdateText(newText);
	}
}
