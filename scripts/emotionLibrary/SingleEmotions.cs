
using Microsoft.Xna.Framework;
using SoulSmithMoves;
using System.Collections.Generic;

namespace SoulSmithEmotions {
    public static class SingleEmotions
    {
        public static Dictionary<EmotionTag, Emotion> CreateDict()
        {
            Dictionary<EmotionTag, Emotion> dict = new Dictionary<EmotionTag, Emotion>();

            dict.Add(EmotionTag.Typeless, Typeless());
            dict.Add(EmotionTag.Joy, Joy());

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

            Emotion emotion = new Emotion(_color);

            return emotion;
        }
    }
}