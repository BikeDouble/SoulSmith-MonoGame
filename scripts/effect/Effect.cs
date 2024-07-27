using SoulSmithMoves;
using SoulSmithEmotions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmithStats;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

public abstract class Effect 
{
    public Effect(EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  string visualizationName = null,
                  double visualizationDelay = 0f,
                  List<Effect> childEffects = null,
                  bool requiresPriority = false,
                  bool swapSenderAndTarget = false) 
    {
        _targetingStyle = targetingStyle;
        _visualizationName = visualizationName;
        _visualizationDelay = visualizationDelay;
        _childEffects = childEffects;
        _requiresPriority = requiresPriority;
        _swapSenderAndTarget = swapSenderAndTarget;
    }

    private EffectTargetingStyle _targetingStyle;

    //For child effects
    private bool _requiresPriority = false;
    private bool _swapSenderAndTarget = false;
    private List<Effect> _childEffects;

    //For visualizations 
    private string _visualizationName = "";
    private EffectVisualization _visualization = null;
    private double _visualizationDelay = 0f;

    public virtual EffectRequest GenerateEffectRequest(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        return null;
    }

    public void LoadVisualizationSprite(TrackedResource<CanvasItem_TransformationRules> sprite)
    {
        _visualization?.LoadSprite(sprite);
    }

    //
    // Damage calculations
    //

    protected static int CalculateRawDamageOutput(int magnitude, DamageType damageType)
    {
        if (magnitude <= 0)
        {
            return 0;
        }

        int retDamage;

        switch (damageType)
        {
            case DamageType.Hit:
                retDamage = CalculateHitDamageOutput(magnitude, damageType);
                break;
            case DamageType.Essence:
                retDamage = CalculateEssenceDamageOutput(magnitude, damageType);
                break;
            default:
                retDamage = 0;
                break;
        }

        return retDamage;
    }

    private static int CalculateHitDamageOutput(int damage, DamageType damageType)
    {
        damage = DamageRoll(damage);
        return damage;
    }

    private static int CalculateEssenceDamageOutput(int damage, DamageType damageType)
    {
        return damage;
    }

    private static int DamageRoll(int damage)
    {
        const double RANGE = 0.2f;
        double coef = Rand.RandDoubleAroundOne(RANGE);
        int result = (int)(coef * damage);
        if (result <= 0)
        {
            result = 1;
        }
        return result;
    }

    public EffectVisualization Visualization { get { return _visualization; } }
    public EffectTargetingStyle TargetingStyle { get { return _targetingStyle; } }
    public double VisualizationDelay { get { return _visualizationDelay; } }
    public ReadOnlyCollection<Effect> ChildEffects { get { return _childEffects?.AsReadOnly(); } }
    public bool RequiresPriority { get { return _requiresPriority; } }
    public bool SwapSenderAndTarget { get { return _swapSenderAndTarget; } }
    public string VisualizationName { get { return _visualizationName; } }
}
