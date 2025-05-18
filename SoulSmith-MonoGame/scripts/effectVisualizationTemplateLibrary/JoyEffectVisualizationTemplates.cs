
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
        public const string JOYPELLETNAME = "JoyPellet";
        public const string JOYPASSIVEPOPNAME = "JoyPassivePop";

        //Sprite names
        public const string JOYPELLETSPRITENAME = "joyPellet";
        public const string JOYPASSIVEPOPSPRITENAME = "joyPassivePop";

        public static Dictionary<string, EffectVisualizationTemplate> CreateDict()
        {
            List<Dictionary<string, EffectVisualizationTemplate>> dicts = new List<Dictionary<string, EffectVisualizationTemplate>>();

            dicts.Add(JoySingleVisualizations());

            return EffectVisualizationTemplateLibrary.MergeDictionaries(dicts);
        }

        private static Dictionary<string, EffectVisualizationTemplate> JoySingleVisualizations()
        {
            Dictionary<string, EffectVisualizationTemplate> dict = new();

            dict.TryAdd(JOYPELLETNAME, JoyPellet());
            dict.TryAdd(JOYPASSIVEPOPNAME, JoyPassivePop());

            return dict;
        }

        private static EffectVisualizationTemplate JoyPellet()
        {
            return EffectVisualizationTemplate.StraightMissile(
                AssetLoader.GetSprite(JOYPELLETSPRITENAME),
                1);
        }

        private static EffectVisualizationTemplate JoyPassivePop()
        {
            return EffectVisualizationTemplate.GrowAndFadeOnTarget(
                AssetLoader.GetSprite(JOYPASSIVEPOPNAME),
                0.25f,
                0);
        }
    }
}