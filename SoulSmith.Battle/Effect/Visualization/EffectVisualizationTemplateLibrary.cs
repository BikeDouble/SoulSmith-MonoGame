
using System;
using System.Collections.Generic;
using SoulSmith.Core;
using SoulSmith.Collections;

namespace SoulSmith.Battle.Effect.Visualization
{
    public static class EffectVisualizationTemplateLibrary
    {
        public static Dictionary<string, EffectVisualizationTemplate> CreateDict()
        {
            List<Dictionary<string, EffectVisualizationTemplate>> dicts = new List<Dictionary<string, EffectVisualizationTemplate>>();

            dicts.Add(JoyEffectVisualizationTemplates.CreateDict());
            dicts.Add(TypelessEffectVisualizationTemplates.CreateDict());

            return MergeDictionaries(dicts);
        }

        public static Dictionary<string, EffectVisualizationTemplate> MergeDictionaries(List<Dictionary<string, EffectVisualizationTemplate>> dicts)
        {
            return DictionaryUtilities.MergeDictionaries<string, EffectVisualizationTemplate>(dicts);
        }
    }
}