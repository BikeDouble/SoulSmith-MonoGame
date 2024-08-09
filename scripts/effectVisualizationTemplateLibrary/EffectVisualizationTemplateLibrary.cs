
using System;
using System.Collections.Generic;

namespace SoulSmithMoves
{
    public static class EffectVisualizationTemplateLibrary
    {
        public static Dictionary<string, EffectVisualizationTemplate> CreateDict(AssetLoader assetLoader)
        {
            List<Dictionary<string, EffectVisualizationTemplate>> dicts = new List<Dictionary<string, EffectVisualizationTemplate>>();

            dicts.Add(JoyEffectVisualizationTemplates.CreateDict(assetLoader));
            dicts.Add(TypelessEffectVisualizationTemplates.CreateDict(assetLoader));

            return MergeDictionaries(dicts);
        }

        public static Dictionary<string, EffectVisualizationTemplate> MergeDictionaries(List<Dictionary<string, EffectVisualizationTemplate>> dicts)
        {
            return DictionaryUtilities.MergeDictionaries<string, EffectVisualizationTemplate>(dicts);
        }
    }
}