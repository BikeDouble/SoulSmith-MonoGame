
using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;

public partial class UnitSprite : CanvasItem_TransformationRules
{
    public const int SPRITEIDLESTATE = 0;
    public const int SPRITEATTACKSTATE = 1;

    public const double ATTACKANIMATIONDURATION = 2;
    public const double HURTANIMATIONDURATION = 2;
    public const double DEATHANIMATIONDURATION = 2;

    private double _animationTime = -1;


    public void PlayAttackAnimation()
    {
        _animationTime = ATTACKANIMATIONDURATION;

        UpdateState(SPRITEATTACKSTATE);
    }

    public void PlayIdleAnimation()
    {
        UpdateState(SPRITEIDLESTATE);
    }

    public override void Process(double delta)
    {
        if (_animationTime > 0)
        {
            _animationTime -= delta;

            if (_animationTime <= 0)
                UpdateState(SPRITEIDLESTATE);
        }

        base.Process(delta);
    }

    public UnitSprite(UnitSprite other) : base(other) { }

    public UnitSprite(CanvasItem_TransformationRules other) : base(other) 
    {
        UpdateState(SPRITEIDLESTATE);
    }
}
