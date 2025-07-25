
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
using Microsoft.Xna.Framework;

namespace SoulSmith.Units;
public class UnitUIHealthBar : CanvasObject
{
	public const string FONTKEY = "Fonts/Raleway/Medium";
    public readonly static DrawableResourceKey EMPTYBARRESOURCEKEY = new DrawableResourceKey("Textures/UI/Units/HealthBar/Empty", "texture");
    public readonly static DrawableResourceKey HEALTHFILLERRESOURCEKEY = new DrawableResourceKey("Textures/UI/Units/HealthBar/HealthFiller", "texture");
    public readonly static DrawableResourceKey DECAYFILLERRESOURCEKEY = new DrawableResourceKey("Textures/UI/Units/HealthBar/DecayFiller", "texture");
    public readonly static Vector2 HEALTHFILLEROFFSET = new Vector2(0, 256);
	public readonly static Vector2 DECAYFILLEROFFSET = new Vector2(0, -256);
	public readonly static Vector2 FILLERDIMENSIONS = new Vector2(112, 512);

	//Children
	private CanvasObject _emptyBar;
	private ScissorRect _healthFiller;
	private ScissorRect _decayFiller;

	public UnitUIHealthBar(Position position = null) : base(position)
	{
        //Backboard
        IDrawableResource emptyBarResource = DrawHelpers.GetDrawableResource(EMPTYBARRESOURCEKEY);
        _emptyBar = new CanvasObject(null, emptyBarResource);
		_emptyBar.SetOriginPlacement(OriginPlacement.Center);
        AddChild(_emptyBar);

        // Health Filler
        IDrawableResource healthFillerResource = DrawHelpers.GetDrawableResource(HEALTHFILLERRESOURCEKEY);
		_healthFiller = new ScissorRect(new Position((int)HEALTHFILLEROFFSET.X, (int)HEALTHFILLEROFFSET.Y, 1, 1, 0, 1), (int)FILLERDIMENSIONS.X, (int)FILLERDIMENSIONS.Y, healthFillerResource);
		_healthFiller.SetOriginPlacement(OriginPlacement.BottomMiddle);
		AddChild(_healthFiller);

        // Decay Filler
        IDrawableResource decayFillerResource = DrawHelpers.GetDrawableResource(DECAYFILLERRESOURCEKEY);
		_decayFiller = new ScissorRect(new Position((int)DECAYFILLEROFFSET.X, (int)DECAYFILLEROFFSET.Y, 1, 1, 0, 2), (int)FILLERDIMENSIONS.X, (int)FILLERDIMENSIONS.Y, decayFillerResource);
		_decayFiller.SetOriginPlacement(OriginPlacement.TopMiddle);
        AddChild(_decayFiller);
	}

	public void Update(IReadOnlyUnit unit)
	{
		int curHealth = unit.GetModStat(StatType.CurHealth);
		int maxHealth = unit.GetModStat(StatType.MaxHealth);
		float healthPercent = (float)curHealth / maxHealth;
		UpdateHealthFiller(healthPercent);

		int curDecay = unit.GetModStat(StatType.CurDecay);
		float decayPercent = (float)curDecay / maxHealth;
		UpdateDecayFiller(decayPercent);
    }

	private void UpdateHealthFiller(float healthPercent)
	{
		_healthFiller.Height = (int)(FILLERDIMENSIONS.Y * healthPercent);
	}

    private void UpdateDecayFiller(float decayPercent)
    {
        _decayFiller.Height = (int)(FILLERDIMENSIONS.Y * decayPercent);
    }
}
