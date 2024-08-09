using System.Collections.Generic;
using SoulSmithStats;
using SoulSmithEmotions;
using System.Collections.ObjectModel;

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
        PredeterminedGlobalTrigger
    }

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
    
    public struct EffectInput
    {
        public EffectInput(Effect effect, IReadOnlyUnit sender, IReadOnlyUnit target = null, IList<float> specialArgs = null)
        {
            Effect = effect;
            Sender = sender;
            Target = target;

            if (specialArgs != null)
            {
                SpecialArgs = new ReadOnlyCollection<float>(specialArgs);
            }
        }

        public void SwapSenderAndTarget()
        {
            IReadOnlyUnit temp = Sender;
            Sender = Target;
            Target = temp;
        }

        public Effect Effect;
        public IReadOnlyUnit Sender;
        public IReadOnlyUnit Target;
        public ReadOnlyCollection<float> SpecialArgs = null;
    }

    public struct MoveInput
    {
        public Move Move;
        public IReadOnlyUnit Sender;
        public IReadOnlyUnit Target;
    }
}

