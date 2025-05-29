using SoulSmith;
using Microsoft.Xna.Framework;
using SoulSmith.Collections;

namespace SoulSmith.Emotion
{
    public static class EmotionLibrary
    {
        public static Dictionary<EmotionTag.EmotionTag, Emotion> CreateDict()
        {
            List<Dictionary<EmotionTag.EmotionTag, Emotion>> dicts = new List<Dictionary<EmotionTag.EmotionTag, Emotion>>();

            dicts.Add(SingleEmotions.CreateDict());

            return MergeDictionaries(dicts);
        }

        public static Dictionary<EmotionTag.EmotionTag, Emotion> MergeDictionaries(List<Dictionary<EmotionTag.EmotionTag, Emotion>> dicts)
        {
            return DictionaryUtilities.MergeDictionaries<EmotionTag.EmotionTag, Emotion>(dicts);
        }
    }
}