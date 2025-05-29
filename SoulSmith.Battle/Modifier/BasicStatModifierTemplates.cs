
using System;
using System.Collections.Generic;
using SoulSmith.Battle;
using SoulSmith.UnitStats;
using SoulSmith.EmotionTag;

namespace SoulSmith.Battle.Modifier
{
    public static class BasicStatModifierTemplates
    {
        // Visualization names
        public const string BASICATTACKSTATICMODIFIERNAME = "AttackBasicStatic";

        //Icon names
        private const string ATTACKSTATICICONNAME = "AttackStatModIcon";

        public static Dictionary<string, ModifierTemplate> CreateDict()
        {
            List<Dictionary<string, ModifierTemplate>> dicts = new List<Dictionary<string, ModifierTemplate>>();

            dicts.Add(StaticStatModifierTemplates());

            return ModifierTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, ModifierTemplate> StaticStatModifierTemplates()
        {
            Dictionary<string, ModifierTemplate> dict = new();

            dict.TryAdd(BASICATTACKSTATICMODIFIERNAME, AttackStatic());

            return dict;
        }

        private static ModifierTemplate AttackStatic()
        {
            return ModifierTemplate.BasicStaticStatModifier(StatType.Attack,
                BASICATTACKSTATICMODIFIERNAME,
                ATTACKSTATICICONNAME);
        }
    }
}