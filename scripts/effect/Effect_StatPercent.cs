using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;
using System;
using System.Collections.Generic;

public partial class Effect_StatPercent : Effect
{
    private StatType _stat; //Which stat effects effect
    private double _statPercent; //How much of the stat effects the effect

    public Effect_StatPercent(StatType stat,
                  double statPercent,
                  EffectTargetingStyle targetingStyle = EffectTargetingStyle.MoveTarget,
                  string visualizationName = null,
                  double visualizationDelay = 0f,
                  List<Effect> childEffects = null,
                  bool requiresPriority = false,
                  bool swapSenderAndTarget = false) 
                  : base(targetingStyle, visualizationName, visualizationDelay, childEffects, requiresPriority, swapSenderAndTarget)
    {
        _stat = stat;
        _statPercent = statPercent;
    }

    public int CalculateMagnitude(Unit statEffector)
    {
        int magnitude = statEffector.Stats.GetModStat(_stat);
        magnitude = (int)(magnitude * _statPercent);
        return magnitude;
    }
}
