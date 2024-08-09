
using System;
using System.Collections.Generic;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;
using SoulSmithModifiers;

namespace SoulSmithMoves
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

        public static Dictionary<string, MoveTemplate> CreateDict(AssetLoader assetLoader)
        {
            List<Dictionary<string, MoveTemplate>> dicts = new List<Dictionary<string, MoveTemplate>>();

            dicts.Add(BasicMoves(assetLoader));

            return MoveTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, MoveTemplate> BasicMoves(AssetLoader assetLoader)
        {
            Dictionary<string, MoveTemplate> dict = new();

            dict.TryAdd(TYPELESSHITTEMPLATENAME, Hit());
            dict.TryAdd(TYPELESSDOUBLEHITTEMPLATENAME, DoubleHit());
            dict.TryAdd(ATTACKUPTEMPLATENAME, AttackUp(assetLoader));

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

        private static MoveTemplate AttackUp(AssetLoader assetLoader)
        {
            List<EffectTemplate> effects = new List<EffectTemplate>();
            ModifierTemplate modifier = assetLoader.GetModifierTemplate(ATTACKUPMODIFIERTEMPLATENAME);
            Dictionary<ModifierFloatArgType, float> modArgs = new Dictionary<ModifierFloatArgType, float>
            {
                { ModifierFloatArgType.AddMod, 4f }
            };

            EffectTemplate effect = EffectTemplate.Modifier(
                modifier,
                modArgs,
                EffectTargetingStyle.MoveTarget,
                null);
            effects.Add(effect);

            MoveTemplate move = new MoveTemplate(ATTACKUPFRIENDLYNAME, ATTACKUPFRIENDLYDESCRIPT, effects, MoveTargetingStyle.AllyOrSelf, EmotionTag.Typeless);

            return move;
        }
    }
}
