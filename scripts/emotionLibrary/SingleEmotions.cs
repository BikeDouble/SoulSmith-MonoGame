
using Microsoft.Xna.Framework;
using SoulSmithModifiers;
using SoulSmithMoves;
using SoulSmithStats;
using System.Collections.Generic;

namespace SoulSmithEmotions {
    public static class SingleEmotions
    {
        public static Dictionary<EmotionTag, Emotion> CreateDict(IAssetLoadOnly assetLoader)
        {
            Dictionary<EmotionTag, Emotion> dict = new Dictionary<EmotionTag, Emotion>();

            dict.Add(EmotionTag.Typeless, Typeless(assetLoader));
            dict.Add(EmotionTag.Joy, Joy(assetLoader));

            return dict;
        }

        private static Emotion Typeless(IAssetLoadOnly assetLoader)
        {
            Color _color = new Color(100, 100, 100, 255);

            Emotion emotion = new Emotion(_color);

            return emotion;
        }

        private static Emotion Joy(IAssetLoadOnly assetLoader)
        {
            Color _color = new Color(245, 188, 0, 255);

            List<ModifierTemplateWithArgs> mofifierTemplates = new();
            Dictionary<ModifierFloatArgType, float> modArgs = new();
            modArgs.TryAdd(ModifierFloatArgType.StatType, (float)StatType.Attack);
            modArgs.TryAdd(ModifierFloatArgType.StatPercent, 0.2f);
            ModifierTemplateWithArgs modifierTemplate = new ModifierTemplateWithArgs(
                assetLoader.GetModifierTemplate(EffectOnHitModifierTemplates.JOYESSENCEDAMAGEONHITNAME),
                modArgs);
            mofifierTemplates.Add(modifierTemplate);

            Emotion emotion = new Emotion(_color, mofifierTemplates);

            return emotion;
        }
    }
}