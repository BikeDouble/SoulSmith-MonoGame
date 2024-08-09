
using System;
using System.Collections.Generic;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;

namespace SoulSmithModifiers
{
    public static class EffectOnHitModifierTemplates
    {
        // Modifier names
        public const string JOYESSENCEDAMAGEONHITNAME = "JoyEssenceDamageOnHit";

        // Visualization names
        public const string JOYESSENCEDAMAGEONHITVISNAME = "JoyEssenceDamageOnHit";

        // Icon names
        public const string JOYESSENCEDAMAGEONHITICONNAME = null;

        public static Dictionary<string, ModifierTemplate> CreateDict(AssetLoader assetLoader)
        {
            List<Dictionary<string, ModifierTemplate>> dicts = new List<Dictionary<string, ModifierTemplate>>();

            dicts.Add(EssenceDamageOnHitModifierTemplates(assetLoader));

            return ModifierTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, ModifierTemplate> EssenceDamageOnHitModifierTemplates(AssetLoader assetLoader)
        {
            Dictionary<string, ModifierTemplate> dict = new();

            dict.TryAdd(JOYESSENCEDAMAGEONHITNAME, JoyEssenceDamageOnHit(assetLoader));

            return dict;
        }

        private static ModifierTemplate JoyEssenceDamageOnHit(AssetLoader assetLoader)
        {
            return ModifierTemplate.SpecialArgStatBasedEssenceDamageOnHitModifier(
                JOYESSENCEDAMAGEONHITNAME,
                JOYESSENCEDAMAGEONHITICONNAME,
                JOYESSENCEDAMAGEONHITVISNAME,
                0.2f);
        }
    }
}