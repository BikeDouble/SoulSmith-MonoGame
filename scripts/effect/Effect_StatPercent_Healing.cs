using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;
using System;
using System.Collections.Generic;

public partial class Effect_StatPercent_Healing : Effect_StatPercent
{
    public Effect_StatPercent_Healing(StatType stat,
                  double statPercent,
                  EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  string visualizationName = null,
                  double visualizationDelay = 0f,
                  List<Effect> childEffects = null,
                  bool requiresPriority = false,
                  bool swapSenderAndTarget = false)
                  : base(stat, statPercent, targetingStyle, visualizationName, visualizationDelay, childEffects, requiresPriority, swapSenderAndTarget)

    {

    }

    public override EffectRequest GenerateEffectRequest(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        int rawHealing = CalculateMagnitude(sender);
        int healing = rawHealing;
        return new EffectRequest(sender, target, healing);
    }
}
