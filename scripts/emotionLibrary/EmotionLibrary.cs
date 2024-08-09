using System;
using System.Collections.Generic;
using SoulSmithMoves;


namespace SoulSmithEmotions
{
    public static class EmotionLibrary
    {
        public static Dictionary<EmotionTag, Emotion> CreateDict(IAssetLoadOnly assetLoader)
        {
            List<Dictionary<EmotionTag, Emotion>> dicts = new List<Dictionary<EmotionTag, Emotion>>();

            dicts.Add(SingleEmotions.CreateDict(assetLoader));

            return MergeDictionaries(dicts);
        }

        public static Dictionary<EmotionTag, Emotion> MergeDictionaries(List<Dictionary<EmotionTag, Emotion>> dicts)
        {
            return DictionaryUtilities.MergeDictionaries<EmotionTag, Emotion>(dicts);
        }
    }
}