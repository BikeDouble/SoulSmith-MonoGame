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

    public static class MoveHelpers
    {
        public static Effect StatHit(
            double percent,
            StatType stat,
            string visualizationName = null,
            double visualizationDelay = 0f)
        {
            List<Effect> childEffects = new List<Effect>();
            childEffects.Add(new Effect_Trigger_OnHit());
            childEffects.Add(new Effect_Trigger(EffectTrigger.OnBeingHit, EffectTargetingStyle.ParentTarget));

            Effect effect = new Effect_StatPercent_Damage(DamageType.Hit,
                                                          stat,
                                                          percent,
                                                          EffectTargetingStyle.MoveTarget,
                                                          visualizationName,
                                                          visualizationDelay,
                                                          childEffects
                                                          );
            return effect;
        }

        //Returns an attack that deals (percent * attacker's attack stat)
        public static Effect AttackHit(
            double percent,
            string visualizationName = null,
            double visualizationDelay = 0f)
        {
            List<Effect> childEffects = new List<Effect>();
            childEffects.Add(new Effect_Trigger_OnHit());
            childEffects.Add(new Effect_Trigger(EffectTrigger.OnBeingHit, EffectTargetingStyle.ParentTarget));

            Effect effect = new Effect_StatPercent_Damage(DamageType.Hit,
                                                          StatType.Attack,
                                                          percent,
                                                          EffectTargetingStyle.MoveTarget,
                                                          visualizationName,
                                                          visualizationDelay,
                                                          childEffects
                                                          );

            return effect;
        }

    }
}

