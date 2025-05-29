using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effect
{
    public enum EffectTrigger
    {
        None,
        OnMoveBegin,
        OnMoveEnd,
        OnTurnBegin,
        OnTurnEnd,
        OnRoundEnd,
        OnRoundBegin,
        OnUnitDeath
    }
}
