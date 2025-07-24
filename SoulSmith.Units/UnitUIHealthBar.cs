
using System;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Battle.Moves;
using SoulSmith.UnitStats;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Object.Canvas;
using SoulSmith.Battle;
using SoulSmith.Asset;
using SoulSmith.Drawing.Text;

namespace SoulSmith.Units;
public class UnitUIHealthBar : CanvasObject
{
	public const string FONTKEY = "Fonts/Raleway/Medium";

	//Children
	private CanvasObject _backboard;
	private CanvasObject _background;
	private CanvasObject _healthbar;
	private CanvasObject _healthText = null;

	public UnitUIHealthBar(Position position = null) : base(position)
	{
		IDrawableResource simpleTextInstance = DrawHelpers.GetDrawableResource(FONTKEY, "simpletextresource");
		_healthText = new CanvasObject(null, simpleTextInstance);
		AddChild(_healthText);
	}

	public void Update(IReadOnlyUnitStats stats)
	{
		int curHealth = stats.GetModStat(StatType.CurHealth);
		int maxHealth = stats.GetModStat(StatType.MaxHealth);
		string newText = curHealth + "/" + maxHealth;
		_healthText.UpdateResourceState(newText);
	}
}
