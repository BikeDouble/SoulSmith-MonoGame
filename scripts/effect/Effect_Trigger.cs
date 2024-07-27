using SoulSmithMoves;
using SoulSmithEmotions;
using System;
using System.Collections.Generic;

public partial class Effect_Trigger : Effect
{
    private EffectTrigger _trigger;

    public Effect_Trigger(EffectTrigger trigger,
                          EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                          string visualizationName = null,
                          double visualizationDelay = 0f,
                          List<Effect> childEffects = null,
                          bool requiresPriority = false,
                          bool swapSenderAndTarget = false)
                          : base(targetingStyle, visualizationName, visualizationDelay, childEffects, requiresPriority, swapSenderAndTarget)

    {
        _trigger = trigger;
    }

    public override EffectRequest GenerateEffectRequest(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        return new EffectRequest(sender, target, _trigger);
    }
}
