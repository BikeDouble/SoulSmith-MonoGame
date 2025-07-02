using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SoulSmith;
using SoulSmith.Battle.Modifier;
using SoulSmith.UnitStats;

namespace SoulSmith.Emotion {
    public static class SingleEmotions
    {
        public static Dictionary<EmotionTag.EmotionTag, Emotion> CreateDict()
        {
            Dictionary<EmotionTag.EmotionTag, Emotion> dict = new Dictionary<EmotionTag.EmotionTag, Emotion>();

            dict.Add(EmotionTag.EmotionTag.Typeless, Typeless());
            dict.Add(EmotionTag.EmotionTag.Joy, Joy());

            return dict;
        }

        private static Emotion Typeless()
        {
            Color _color = new Color(100, 100, 100, 255);

            Emotion emotion = new Emotion(_color);

            return emotion;
        }

        private static Emotion Joy()
        {
            Color _color = new Color(245, 188, 0, 255);

            List<IModifier> mofifierTemplates = new();
            /*ModifierTemplateWithArgs modifierTemplate = new ModifierTemplateWithArgs( //TODO
                MasterAssetLoader.GetModifierTemplate(EffectOnHitModifierTemplates.JOYESSENCEDAMAGEONHITNAME),
                modArgs);
            mofifierTemplates.Add(modifierTemplate);*/

            Emotion emotion = new Emotion(_color, mofifierTemplates);

            return emotion;
        }
    }
}