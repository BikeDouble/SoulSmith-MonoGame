using System.Collections.Generic;
using SoulSmithStats;
using SoulSmithEmotions;

namespace SoulSmithMoves
{

    public enum EffectType
    {
        Damage,
        OnHit,
        Healing,
        Modifier
    }

    public enum DamageType
    {
        Hit,
        Essence
    }

    public enum MoveTargetingStyle
    {
        Enemy,
        AllyOrSelf,
        Ally
    }

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
        DeterminedPreOffer
    }

    public enum EffectTrigger
    {
        None,
        OnHitting,
        OnBeingHit,
        OnMoveBegin,
        OnMoveEnd,
        OnTurnBegin,
        OnTurnEnd,
        OnRoundEnd,
        OnRoundBegin
    }
    
    public struct EffectInput
    {
        public EffectInput(Effect effect, Unit sender, Unit target = null)
        {
            Effect = effect;
            Sender = sender;
            Target = target;
        }

        public void SwapSenderAndTarget()
        {
            Unit temp = Sender;
            Sender = Target;
            Target = temp;
        }

        public Effect Effect;
        public Unit Sender;
        public Unit Target;
    }

    public struct MoveInput
    {
        public Move Move;
        public Unit Sender;
        public Unit Target;
    }
}

