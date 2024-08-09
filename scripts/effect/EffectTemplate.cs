
using SoulSmithModifiers;
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
            TargetingStyle = targetingStyle;
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
                GenerateTriggerFunc(trigger),
                targetingStyle,
                childEffects,
                visName,
                visDelay,
                priority,
                swapSenderAndTarget);
        }

        public static EffectTemplate Modifier(
            ModifierTemplate modifier,
            IDictionary<ModifierFloatArgType, float> modifierArgs,
            EffectTargetingStyle targetingStyle,
            IEnumerable<EffectTemplate> childEffects = null,
            string visName = "none",
            float visDelay = 0,
            bool priority = false,
            bool swapSenderAndTarget = false)
        {
            return new EffectTemplate(
                GenerateModFunc(modifier, modifierArgs),
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
                GenerateStatHitFunc(percent, stat),
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

        public static EffectTemplate SpecialArgFlatEssenceDamage(
            IEnumerable<EffectTemplate> childEffects = null,
            string visName = "none",
            float visDelay = 0,
            bool priority = false)
        {
            return new EffectTemplate(
                SpecialArgsFlatEssenceDamageFunc,
                EffectTargetingStyle.PredeterminedGlobalTrigger,
                childEffects,
                visName,
                visDelay,
                priority);
        }

        public static EffectTemplate SpecialArgStatBasedEssenceDamage(
            IEnumerable<EffectTemplate> childEffects = null,
            string visName = "none",
            float visDelay = 0,
            bool priority = false)
        {
            return new EffectTemplate(
                SpecialArgsStatBasedEssenceDamageFunc,
                EffectTargetingStyle.PredeterminedGlobalTrigger,
                childEffects,
                visName,
                visDelay,
                priority);
        }

        private static Func<GenerateEffectRequestArgs, EffectRequest> SpecialArgsFlatEssenceDamageFunc = (args) =>
        {
            int damage = (int)args.SpecialArgs[0];

            return new EffectRequest(
                args.Sender,
                args.Target,
                DamageType.Essence,
                damage);
        };

        private static Func<GenerateEffectRequestArgs, EffectRequest> SpecialArgsStatBasedEssenceDamageFunc = (args) =>
        {
            StatType statType = ModifierTemplate.FloatToStatType(args.SpecialArgs[0]);
            float statPercent = args.SpecialArgs[1];

            int damage = (int)(statPercent * args.Sender.GetModStat(statType));

            return new EffectRequest(
                args.Sender,
                args.Target,
                DamageType.Essence,
                damage);
        };

        public static Func<GenerateEffectRequestArgs, EffectRequest> GenerateStatHitFunc(
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

        public static Func<GenerateEffectRequestArgs, EffectRequest> GenerateTriggerFunc(
            EffectTrigger trigger)
        {
            return (args) => (new EffectRequest(
                args.Sender,
                args.Target,
                trigger,
                args.ChildEffects));
        }

        public static Func<GenerateEffectRequestArgs, EffectRequest> GenerateModFunc(
            ModifierTemplate mod,
            IDictionary<ModifierFloatArgType, float> modArgs = null)
        {
            return (args) => (new EffectRequest(
                args.Sender,
                args.Target,
                mod,
                modArgs,
                args.ChildEffects));
        }

        public static int DamageRoll(int rawDamage)
        {
            return (int)(rawDamage * Rand.RandDoubleAroundOne(0.2));
        }

    }

    public class GenerateEffectRequestArgs
    {
        public IReadOnlyUnit Sender;
        public IReadOnlyUnit Target;
        public EffectResult ParentEffectResult = null;
        public ReadOnlyCollection<Effect> ChildEffects = null;
        public ReadOnlyCollection<float> SpecialArgs = null;
    }
}
