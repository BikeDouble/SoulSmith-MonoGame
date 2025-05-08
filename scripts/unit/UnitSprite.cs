
using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public partial class UnitSprite : CanvasItem_TransformationRules, IReadOnlyUnitSprite
{
    public const double ANIMATIONDESYNCFACTORRADIUS = 0.05f;

    public const int SPRITEIDLESTATE = 0;
    public const int SPRITEATTACKSTATE = 1;
    public const int SPRITEHURTSTATE = 2;
    public const int SPRITEDEATHSTATE = 3;

    public const double ATTACKANIMATIONDURATION = 2;
    public const double HURTANIMATIONDURATION = 2;
    public const double DEATHANIMATIONDURATION = 2;

    private double _animationTime = -1;
    private ReadOnlyCollection<CanvasTransformationRule> _hPAffectedTransformationRules;

    public void PlayAttackAnimation()
    {
        _animationTime = ATTACKANIMATIONDURATION;

        UpdateState(SPRITEATTACKSTATE);
    }

    public void PlayIdleAnimation()
    {
        UpdateState(SPRITEIDLESTATE);
    }

    public void PlayDeathAnimation()
    {
        _animationTime = DEATHANIMATIONDURATION;

        UpdateState(SPRITEDEATHSTATE);
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
        int curHP = stats.GetModStat(SoulSmithStats.StatType.CurHealth);
        int maxHP = stats.GetModStat(SoulSmithStats.StatType.MaxHealth);
        float remainingHPPercent = (float) (curHP / maxHP);
        List<float> args = new List<float> { remainingHPPercent };

        foreach (var rule in _hPAffectedTransformationRules)
        {
            rule.SetVelocityFuncArgs(args);
        }
    }

    public override void Process(double delta)
    {
        if (_animationTime > 0)
        {
            _animationTime -= delta;

            if (_animationTime <= 0)
            {
                UpdateState(SPRITEIDLESTATE);
                _animationTime = 0;
            }
        }

        base.Process(delta);
    }

    public UnitSprite(UnitSprite other, float animationCoef = 1) : base(other, null, animationCoef) 
    {
        _hPAffectedTransformationRules = GetAllTransformationRulesInChildTreeWithTag("hpaffected");
    }

    public UnitSprite(CanvasItem_TransformationRules other, double animationCoef = 1) : base(other, null, animationCoef) 
    {
        UpdateState(SPRITEIDLESTATE);
        _hPAffectedTransformationRules = GetAllTransformationRulesInChildTreeWithTag("hpaffected");
    }

    public UnitSprite(CanvasItem other, double animationCoef = 1) : base(other, null, animationCoef)
    {
        UpdateState(SPRITEIDLESTATE);
        _hPAffectedTransformationRules = GetAllTransformationRulesInChildTreeWithTag("hpaffected");
    }
}
