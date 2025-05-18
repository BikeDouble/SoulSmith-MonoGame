
using System;
using Microsoft.Xna.Framework.Graphics;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmith.Drawing;

public partial class UnitUIHealthBar : CanvasItem
{
	//Children
	private CanvasItem _backboard;
	private CanvasItem _background;
	private CanvasItem _healthbar;
	private CanvasItem _healthText = null;

	public UnitUIHealthBar(SpriteFont font, CanvasPosition position = null) : base(position)
	{
		_healthText = new CanvasItem(font);
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
