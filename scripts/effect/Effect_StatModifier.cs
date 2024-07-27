using System;
using SoulSmithStats;
using SoulSmithMoves;
using SoulSmithEmotions;
using System.Collections.Generic;

public partial class Effect_StatModifier : Effect
{
    private StatModifier _statModifier;

    public Effect_StatModifier(StatModifier statModifier,
                  EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  string visualizationName = null,
                  double visualizationDelay = 0f,
                  List<Effect> childEffects = null,
                  bool requiresPriority = false,
                  bool swapSenderAndTarget = false) : base(targetingStyle, visualizationName, visualizationDelay, childEffects, requiresPriority, swapSenderAndTarget)
    {
        _statModifier = statModifier;
    }

    public override EffectRequest GenerateEffectRequest(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        Modifier modifier = new Modifier_Stat(_statModifier);
        EffectRequest request = new EffectRequest(sender, target, modifier);
        return request;
    }
}