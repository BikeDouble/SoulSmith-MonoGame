
using System;
using System.Collections.Generic;
using SoulSmith.Battle.Effect.Visualization;

namespace SoulSmith.Battle.Modifier
{
    public static class EffectOnHitModifierTemplates
    {
        // Modifier names
        public const string JOYESSENCEDAMAGEONHITNAME = "JoyEssenceDamageOnHit";

        // Visualization names
        public const string JOYESSENCEDAMAGEONHITVISNAME = JoyEffectVisualizationTemplates.JOYPASSIVEPOPNAME;

        // Icon names
        public const string JOYESSENCEDAMAGEONHITICONNAME = "";

        public static Dictionary<string, ModifierTemplate> CreateDict()
        {
            List<Dictionary<string, ModifierTemplate>> dicts = new List<Dictionary<string, ModifierTemplate>>();

            dicts.Add(EssenceDamageOnHitModifierTemplates());

            return ModifierTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, ModifierTemplate> EssenceDamageOnHitModifierTemplates()
        {
            Dictionary<string, ModifierTemplate> dict = new();

            dict.TryAdd(JOYESSENCEDAMAGEONHITNAME, JoyEssenceDamageOnHit());

            return dict;
        }

        private static ModifierTemplate JoyEssenceDamageOnHit()
        {
            return ModifierTemplate.SpecialArgStatBasedEssenceDamageOnHitModifier(
                JOYESSENCEDAMAGEONHITNAME,
                JOYESSENCEDAMAGEONHITICONNAME,
                JOYESSENCEDAMAGEONHITVISNAME,
                0.5f);
        }
    }
}