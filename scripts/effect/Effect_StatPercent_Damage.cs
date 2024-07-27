using SoulSmithMoves;
using SoulSmithEmotions;
using System;
using SoulSmithStats;
using System.Collections.Generic;

public partial class Effect_StatPercent_Damage : Effect_StatPercent
{
    private DamageType _damageType;

    public Effect_StatPercent_Damage(DamageType damageType,
                  StatType stat,
                  double statPercent,
                  EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  string visualizationName = null,
                  double visualizationDelay = 0f,
                  List<Effect> childEffects = null,
                  bool requiresPriority = false,
                  bool swapSenderAndTarget = false)
                  : base(stat, statPercent, targetingStyle, visualizationName, visualizationDelay, childEffects, requiresPriority, swapSenderAndTarget)
    {
        _damageType = damageType;
    }

    public override EffectRequest GenerateEffectRequest(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        int magnitude = CalculateMagnitude(sender);
        int rawDamage = CalculateRawDamageOutput(magnitude, _damageType);
        return new EffectRequest(sender, target, _damageType, rawDamage);
    }
}
