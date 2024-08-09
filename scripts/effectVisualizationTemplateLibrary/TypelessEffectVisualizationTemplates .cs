
using System;
using System.Collections.Generic;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;

namespace SoulSmithMoves
{
    public static class TypelessEffectVisualizationTemplates
    {
        // Visualization names
        private const string TYPELESSPELLETNAME = "TypelessPellet";

        //Sprite names
        private const string TYPELESSPELLETSPRITENAME = "TypelessPellet";

        public static Dictionary<string, EffectVisualizationTemplate> CreateDict(AssetLoader assetLoader)
        {
            List<Dictionary<string, EffectVisualizationTemplate>> dicts = new List<Dictionary<string, EffectVisualizationTemplate>>();

            dicts.Add(TypelessSingleVisualizations(assetLoader));

            return EffectVisualizationTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, EffectVisualizationTemplate> TypelessSingleVisualizations(AssetLoader assetLoader)
        {
            Dictionary<string, EffectVisualizationTemplate> dict = new();

            dict.TryAdd(TYPELESSPELLETNAME, TypelessPellet(assetLoader));

            return dict;
        }

        private static EffectVisualizationTemplate TypelessPellet(AssetLoader assetLoader)
        {
            return EffectVisualizationTemplate.StraightMissile(
                assetLoader.GetSprite(TYPELESSPELLETSPRITENAME),
                1);
        }
    }
}