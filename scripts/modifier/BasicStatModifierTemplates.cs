
using System;
using System.Collections.Generic;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;

namespace SoulSmithModifiers
{
    public static class BasicStatModifierTemplates
    {
        // Visualization names
        public const string BASICATTACKSTATICMODIFIERNAME = "AttackBasicStatic";

        //Icon names
        private const string ATTACKSTATICICONNAME = "AttackStatModIcon";

        public static Dictionary<string, ModifierTemplate> CreateDict(AssetLoader assetLoader)
        {
            List<Dictionary<string, ModifierTemplate>> dicts = new List<Dictionary<string, ModifierTemplate>>();

            dicts.Add(StaticStatModifierTemplates(assetLoader));

            return ModifierTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, ModifierTemplate> StaticStatModifierTemplates(AssetLoader assetLoader)
        {
            Dictionary<string, ModifierTemplate> dict = new();

            dict.TryAdd(BASICATTACKSTATICMODIFIERNAME, AttackStatic(assetLoader));

            return dict;
        }

        private static ModifierTemplate AttackStatic(AssetLoader assetLoader)
        {
            return ModifierTemplate.BasicStaticStatModifier(StatType.Attack,
                BASICATTACKSTATICMODIFIERNAME,
                ATTACKSTATICICONNAME);
        }
    }
}