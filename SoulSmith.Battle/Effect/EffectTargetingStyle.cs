using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effect
{
    public enum EffectTargetingStyle
    {
        MoveTarget,
        Self,
        LowestHPEnemy,
        HighestHPEnemy,
        LowestHPAllyOrSelf,
        Attacker,
        ParentTarget,
        ParentSender,
        PredeterminedGlobalTrigger
    }
}
