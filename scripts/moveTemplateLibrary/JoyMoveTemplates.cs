
using System;
using System.Collections.Generic;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;

namespace SoulSmithMoves
{
    public static class JoyMoveTemplates
    {
        // Hit
        private const string JOYHITFRIENDLYNAME = "Laugh";
        private const string JOYHITFRIENDLYDESCRIPT = "Hit an enemy for 100% of your attack stat.";
        private const string JOYHITTEMPLATENAME = "JoyHit";

        // Double Hit
        private const string JOYDOUBLEHITFRIENDLYNAME = "Chuckle";
        private const string JOYDOUBLEHITFRIENDLYDESCRIPT = "Hit an enemy for 40% of your attack stat twice.";
        private const string JOYDOUBLEHITTEMPLATENAME = "JoyDoubleHit";

        // Visualization names
        private const string JOYPELLETVISUALIZATIONNAME = "JoyPellet";

        public static Dictionary<string, MoveTemplate> CreateDict(AssetLoader assetLoader)
        {
            List<Dictionary<string, MoveTemplate>> dicts = new List<Dictionary<string, MoveTemplate>>();

            dicts.Add(BasicMoves());

            return MoveTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, MoveTemplate> BasicMoves()
        {
            Dictionary<string, MoveTemplate> dict = new();

            dict.TryAdd(JOYHITTEMPLATENAME, Hit());
            dict.TryAdd(JOYDOUBLEHITTEMPLATENAME, DoubleHit());

            return dict;
        }

        private static MoveTemplate Hit()
        {
            List<EffectTemplate> effects = new List<EffectTemplate>();
            EffectTemplate effect = EffectTemplate.AttackHit(
                1f,
                EffectTargetingStyle.MoveTarget,
                null,
                JOYPELLETVISUALIZATIONNAME);
            effects.Add(effect);

            MoveTemplate move = new MoveTemplate(
                JOYHITFRIENDLYNAME,
                JOYHITFRIENDLYDESCRIPT,
                effects,
                MoveTargetingStyle.Enemy,
                EmotionTag.Joy);

            return move;
        }

        private static MoveTemplate DoubleHit()
        {

            List<EffectTemplate> effects = new List<EffectTemplate>();
            EffectTemplate effect = EffectTemplate.AttackHit(
                0.4f,
                EffectTargetingStyle.MoveTarget,
                null,
                JOYPELLETVISUALIZATIONNAME);
            effects.Add(effect);

            effect = EffectTemplate.AttackHit(
                0.4f,
                EffectTargetingStyle.MoveTarget,
                null,
                JOYPELLETVISUALIZATIONNAME,
                0.2f);
            effects.Add(effect);

            MoveTemplate move = new MoveTemplate(
                JOYDOUBLEHITFRIENDLYNAME,
                JOYDOUBLEHITFRIENDLYDESCRIPT,
                effects,
                MoveTargetingStyle.Enemy,
                EmotionTag.Joy);
            return move;
        }
    }
}