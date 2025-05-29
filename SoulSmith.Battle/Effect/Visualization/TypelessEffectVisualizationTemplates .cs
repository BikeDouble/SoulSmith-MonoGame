
using System;
using System.Collections.Generic;

namespace SoulSmith.Battle.Effect.Visualization
{
    public static class TypelessEffectVisualizationTemplates
    {
        // Visualization names
        private const string TYPELESSPELLETNAME = "TypelessPellet";

        //Sprite names
        private const string TYPELESSPELLETSPRITENAME = "TypelessPellet";

        public static Dictionary<string, EffectVisualizationTemplate> CreateDict()
        {
            List<Dictionary<string, EffectVisualizationTemplate>> dicts = new List<Dictionary<string, EffectVisualizationTemplate>>();

            dicts.Add(TypelessSingleVisualizations());

            return EffectVisualizationTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, EffectVisualizationTemplate> TypelessSingleVisualizations()
        {
            Dictionary<string, EffectVisualizationTemplate> dict = new();

            dict.TryAdd(TYPELESSPELLETNAME, TypelessPellet());

            return dict;
        }

        private static EffectVisualizationTemplate TypelessPellet()
        {
            return EffectVisualizationTemplate.StraightMissile(
                null, //MasterAssetLoader.GetSprite(TYPELESSPELLETSPRITENAME), TODO
                1);
        }
    }
}