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

    public const bool FORCEATTACKSTATE = true;
    public const bool FORCEHURTSTATE = true;
    public const bool FORCEDEATHSTATE = true;

    public const double ATTACKANIMATIONDURATION = EffectQueue.ATTACKANIMATIONDURATION;
    public const double HURTANIMATIONDURATION = 1;
    public const double DEATHANIMATIONDURATION = EffectQueue.DEATHANIMATIONDURATION;

    public const double MINIMUMTIMEBETWEENHURTANIMATIONSTARTS = 0.5d + HURTANIMATIONDURATION; // Hurt animation can look goofy if played directly after itself

    public const float SCALE = 1f;

    public readonly static Vector2 BASESIZE = new Vector2(256, 256);

    private double _timeInAnimation = -1;
    private double _timeUntilHurtAnimationAllowed = -1;

    public UnitSprite(IDrawableResource sprite, double animationDesyncFactor) : base(null, sprite)
    {
        this.ScaleToSetSize(BASESIZE * SCALE);
        //TODO implement animationDesyncFactor
    }

    public void PlayHurtAnimation()
    {
        if (_timeUntilHurtAnimationAllowed <= 0)
        {
            _timeUntilHurtAnimationAllowed = MINIMUMTIMEBETWEENHURTANIMATIONSTARTS;
            UpdateResourceState(SPRITEHURTSTATE, FORCEHURTSTATE);
            _timeInAnimation = HURTANIMATIONDURATION;
        }
    }

    public void PlayDeathAnimation()
    {
        UpdateResourceState(SPRITEDEATHSTATE, FORCEDEATHSTATE);
        _timeInAnimation = DEATHANIMATIONDURATION;
    }

    public void PlayAttackAnimation()
    {
        UpdateResourceState(SPRITEATTACKSTATE, FORCEATTACKSTATE);
        _timeInAnimation = ATTACKANIMATIONDURATION;
    }

    public override void Process(double delta)
    {
        if (_timeInAnimation > 0)
        {
            _timeInAnimation -= delta;

            if (_timeInAnimation <= 0)
            {
                UpdateResourceState("idle", true);
                _timeInAnimation = -1;
            }
        }

        if (_timeUntilHurtAnimationAllowed > 0)
        {
            _timeUntilHurtAnimationAllowed -= delta;
        }

        base.Process(delta);
    }
}
