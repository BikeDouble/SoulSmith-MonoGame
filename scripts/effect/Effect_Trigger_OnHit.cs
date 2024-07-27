using SoulSmithMoves;
using SoulSmithEmotions;

using System;
using System.Collections.Generic;

public partial class Effect_Trigger_OnHit : Effect_Trigger
{

    public Effect_Trigger_OnHit(
                  string visualizationName = null,
                  double visualizationDelay = 0f,
                  List<Effect> childEffects = null)
                  : base(EffectTrigger.OnHitting, EffectTargetingStyle.ParentTarget, visualizationName, visualizationDelay, childEffects, true, true)

    {

    }

    public override EffectRequest GenerateEffectRequest(Unit sender, Unit target, EffectResult parentEffectResult = null)
    {
        return base.GenerateEffectRequest(sender, target, parentEffectResult);
    }
}
