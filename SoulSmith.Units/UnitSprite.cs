using SoulSmith.Object.Canvas;
using SoulSmith.Battle;
using SoulSmith.Asset;
using SoulSmith.Drawing;
using Microsoft.Xna.Framework;
using SoulSmith.Battle.Effects;

namespace SoulSmith.Units;
public class UnitSprite : CanvasObject, IReadOnlyUnitSprite
{
    public const double ANIMATIONDESYNCFACTORRADIUS = 0.05f;

    public const string SPRITEIDLESTATE = "idle";
    public const string SPRITEATTACKSTATE = "attack";
    public const string SPRITEHURTSTATE = "hurt";
    public const string SPRITEDEATHSTATE = "death";

    public const double ATTACKANIMATIONDURATION = EffectQueue.ATTACKANIMATIONDURATION;
    public const double HURTANIMATIONDURATION = 2;
    public const double DEATHANIMATIONDURATION = EffectQueue.DEATHANIMATIONDURATION;

    public const float WIDTHSCALE = 1.2f;
    public const float HEIGHTSCALE = 1.2f;

    private double _animationTime = -1;

    public UnitSprite(IAssetWrapper<IDrawableResource> sprite, double animationDesyncFactor) : base(null, sprite)
    {
        this.Scale(new Vector2(WIDTHSCALE, HEIGHTSCALE));
        //TODO implement animationDesyncFactor
    }

    public void Update(IReadOnlyUnitStats stats)
    {
        if (stats != null)
        {
            UpdateHP(stats);
        }
    }

    private void UpdateHP(IReadOnlyUnitStats stats)
    {
        /*int curHP = stats.GetModStat(SoulSmithStats.StatType.CurHealth);
        int maxHP = stats.GetModStat(SoulSmithStats.StatType.MaxHealth);
        float remainingHPPercent = (float) (curHP / maxHP);
        List<float> args = new List<float> { remainingHPPercent };

        foreach (var rule in _hPAffectedTransformationRules)
        {
            rule.SetVelocityFuncArgs(args);
        }*/
    }

    public override void Process(double delta)
    {
        if (_animationTime > 0)
        {
            _animationTime -= delta;

            if (_animationTime <= 0)
            {
                UpdateResourceState("idle");
                _animationTime = 0;
            }
        }

        base.Process(delta);
    }
}
