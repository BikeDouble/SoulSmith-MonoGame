
using System;
using System.Collections.Generic;
using SoulSmith.UnitStats;
using SoulSmith.EmotionTag;
using SoulSmith.Battle.Modifier;
using SoulSmith.Battle.Effect;

namespace SoulSmith.Battle.Move
{
    public static class TypelessMoveTemplates
    {
        // Hit
        private const string TYPELESSHITFRIENDLYNAME = "Hit";
        private const string TYPELESSHITFRIENDLYDESCRIPT = "Hit an enemy for 100% of your attack stat.";
        private const string TYPELESSHITTEMPLATENAME = "TypelessHit";

        // Double Hit
        private const string TYPELESSDOUBLEHITFRIENDLYNAME = "Double Hit";
        private const string TYPELESSDOUBLEHITFRIENDLYDESCRIPT = "Hit an enemy for 40% of your attack stat twice.";
        private const string TYPELESSDOUBLEHITTEMPLATENAME = "TypelessDoubleHit";

        // Attack Up
        private const string ATTACKUPFRIENDLYNAME = "Attack Up";
        private const string ATTACKUPFRIENDLYDESCRIPT = "Boost the attack of any unit on your team.";
        private const string ATTACKUPTEMPLATENAME = "TypelessAttackUp";
        private const string ATTACKUPMODIFIERTEMPLATENAME = BasicStatModifierTemplates.BASICATTACKSTATICMODIFIERNAME;

        // Visualization names
        private const string TYPELESSPELLETVISUALIZATIONNAME = "TypelessPellet";

        public static Dictionary<string, MoveTemplate> CreateDict()
        {
            List<Dictionary<string, MoveTemplate>> dicts = new List<Dictionary<string, MoveTemplate>>();

            dicts.Add(BasicMoves());

            return MoveTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, MoveTemplate> BasicMoves()
        {
            Dictionary<string, MoveTemplate> dict = new();

            dict.TryAdd(TYPELESSHITTEMPLATENAME, Hit());
            dict.TryAdd(TYPELESSDOUBLEHITTEMPLATENAME, DoubleHit());
            dict.TryAdd(ATTACKUPTEMPLATENAME, AttackUp());

            return dict;
        }

        private static MoveTemplate Hit()
        {
            List<EffectTemplate> effects = new List<EffectTemplate>();
            EffectTemplate effect = EffectTemplate.AttackHit(
                1f,
                EffectTargetingStyle.MoveTarget,
                null,
                TYPELESSPELLETVISUALIZATIONNAME);
            effects.Add(effect);

            MoveTemplate move = new MoveTemplate(TYPELESSHITFRIENDLYNAME, TYPELESSHITFRIENDLYDESCRIPT, effects);

            return move;
        }

        private static MoveTemplate DoubleHit()
        {

            List<EffectTemplate> effects = new List<EffectTemplate>();
            EffectTemplate effect = EffectTemplate.AttackHit(
                0.4f,
                EffectTargetingStyle.MoveTarget,
                null,
                TYPELESSPELLETVISUALIZATIONNAME);
            effects.Add(effect);

            effect = EffectTemplate.AttackHit(
                0.4f,
                EffectTargetingStyle.MoveTarget,
                null,
                TYPELESSPELLETVISUALIZATIONNAME,
                0.5f);
            effects.Add(effect);

            MoveTemplate move = new MoveTemplate(TYPELESSDOUBLEHITFRIENDLYNAME, TYPELESSDOUBLEHITFRIENDLYDESCRIPT, effects);
            return move;
        }

        private static MoveTemplate AttackUp()
        {
            List<EffectTemplate> effects = new List<EffectTemplate>();
            ModifierTemplate modifier = null; //MasterAssetLoader.GetModifierTemplate(ATTACKUPMODIFIERTEMPLATENAME); TODO
            Dictionary<ModifierFloatArgType, float> modArgs = new Dictionary<ModifierFloatArgType, float>
            {
                { ModifierFloatArgType.AddMod, 0.5f },
                { ModifierFloatArgType.Duration, 3 },
            };

            EffectTemplate effect = EffectTemplate.Modifier(
                modifier,
                modArgs,
                EffectTargetingStyle.MoveTarget,
                null);
            effects.Add(effect);

            MoveTemplate move = new MoveTemplate(ATTACKUPFRIENDLYNAME, ATTACKUPFRIENDLYDESCRIPT, effects, MoveTargetingStyle.AllyOrSelf, EmotionTag.EmotionTag.Typeless);

            return move;
        }
    }
}
