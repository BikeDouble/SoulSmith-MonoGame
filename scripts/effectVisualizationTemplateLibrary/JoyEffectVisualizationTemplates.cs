
using System;
using System.Collections.Generic;
using SoulSmithMoves;
using SoulSmithStats;
using SoulSmithEmotions;

namespace SoulSmithMoves
{
    public static class JoyEffectVisualizationTemplates
    {
        // Visualization names
        private const string JOYPELLETNAME = "JoyPellet";

        //Sprite names
        private const string JOYPELLETSPRITENAME = "joyPellet";

        public static Dictionary<string, EffectVisualizationTemplate> CreateDict(AssetLoader assetLoader)
        {
            List<Dictionary<string, EffectVisualizationTemplate>> dicts = new List<Dictionary<string, EffectVisualizationTemplate>>();

            dicts.Add(JoySingleVisualizations(assetLoader));

            return EffectVisualizationTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, EffectVisualizationTemplate> JoySingleVisualizations(AssetLoader assetLoader)
        {
            Dictionary<string, EffectVisualizationTemplate> dict = new();

            dict.TryAdd(JOYPELLETNAME, JoyPellet(assetLoader));

            return dict;
        }

        private static EffectVisualizationTemplate JoyPellet(AssetLoader assetLoader)
        {
            return EffectVisualizationTemplate.StraightMissile(
                assetLoader.GetSprite(JOYPELLETSPRITENAME),
                1);
        }
    }
}