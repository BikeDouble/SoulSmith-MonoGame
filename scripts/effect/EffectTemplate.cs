
using SoulSmithStats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SoulSmithMoves
{
    public class EffectTemplate
    {
        public EffectTemplate(
            Func<GenerateEffectRequestArgs, EffectRequest> generateEffectRequest,
            EffectTargetingStyle targetingStyle,
            IEnumerable<EffectTemplate> childEffects,
            string visulizationName,
            float visualizationDelay,
            bool priority = false,
            bool swapSenderAndTarget = false) 
        {
            GenerateEffectRequest = generateEffectRequest;
            ChildEffects = childEffects?.ToList().AsReadOnly();
            VisualizationName = visulizationName;
            VisualizationDelay = visualizationDelay;
            RequiresPriority = priority;
            SwapSenderAndTarget = swapSenderAndTarget;
        }

        public bool RequiresPriority { get; }
        public bool SwapSenderAndTarget { get; }
        public EffectTargetingStyle TargetingStyle { get; }
        public ReadOnlyCollection<EffectTemplate> ChildEffects { get; }
        public string VisualizationName { get; } = "none";
        public float VisualizationDelay { get; } = 0;
        public Func<GenerateEffectRequestArgs, EffectRequest> GenerateEffectRequest { get; private set; } = (args) => (new EffectRequest(args.Sender, args.Target));

        public static EffectTemplate Trigger(
            EffectTrigger trigger,
            EffectTargetingStyle targetingStyle,
            IEnumerable<EffectTemplate> childEffects = null,
            string visName = "none",
            float visDelay = 0,
            bool priority = false,
            bool swapSenderAndTarget = false)
        {
            return new EffectTemplate(
                TriggerFunc(trigger),
                targetingStyle,
                childEffects,
                visName,
                visDelay,
                priority,
                swapSenderAndTarget);
        }

        public static EffectTemplate StatModifier(
            StatModifier statModifier,
            int duration,
            EffectTargetingStyle targetingStyle,
            IEnumerable<EffectTemplate> childEffects = null,
            string visName = "none",
            float visDelay = 0,
            bool priority = false,
            bool swapSenderAndTarget = false)
        {
            return new EffectTemplate(
                StatModFunc(statModifier, duration),
                targetingStyle,
                childEffects,
                visName,
                visDelay,
                priority,
                swapSenderAndTarget);
        }

        public static EffectTemplate StatHit(
            float percent,
            StatType stat,
            EffectTargetingStyle targetingStyle,
            IEnumerable<EffectTemplate> childEffects = null,
            string visName = "none",
            float visDelay = 0,
            bool priority = false,
            bool swapSenderAndTarget = false)
        {
            return new EffectTemplate(
                StatHitFunc(percent, stat),
                targetingStyle,
                childEffects,
                visName,
                visDelay,
                priority,
                swapSenderAndTarget);
        }

        public static EffectTemplate AttackHit(
            float percent,
            EffectTargetingStyle targetingStyle,
            IEnumerable<EffectTemplate> childEffects = null,
            string visName = "none",
            float visDelay = 0,
            bool priority = false,
            bool swapSenderAndTarget = false)
        {
            return StatHit(percent, StatType.Attack, targetingStyle, childEffects, visName, visDelay, priority, swapSenderAndTarget);
        }

        public static Func<GenerateEffectRequestArgs, EffectRequest> StatHitFunc(
            float percent,
            StatType stat)
        {
            return (args) => (new EffectRequest(
                args.Sender, 
                args.Target,
                DamageType.Hit,
                DamageRoll((int)(args.Sender.GetModStat(stat) * percent)),
                true,
                args.ChildEffects));
        }

        public static Func<GenerateEffectRequestArgs, EffectRequest> TriggerFunc(
            EffectTrigger trigger)
        {
            return (args) => (new EffectRequest(
                args.Sender,
                args.Target,
                trigger,
                args.ChildEffects));
        }

        public static Func<GenerateEffectRequestArgs, EffectRequest> StatModFunc(
            StatModifier statMod,
            int duration = -1)
        {
            return (args) => (new EffectRequest(
                args.Sender,
                args.Target,
                new Modifier_Stat(statMod, duration),
                args.ChildEffects));
        }

        public static int DamageRoll(int rawDamage)
        {
            return (int)(rawDamage * Rand.RandDoubleAroundOne(0.2));
        }

    }

    public class GenerateEffectRequestArgs
    {
        public Unit Sender;
        public Unit Target;
        public EffectResult ParentEffectResult = null;
        public ReadOnlyCollection<Effect> ChildEffects = null;
    }
}
