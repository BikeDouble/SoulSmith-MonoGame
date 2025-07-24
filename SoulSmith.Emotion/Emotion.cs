using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmith.Battle.Modifiers;
using SoulSmith.EmotionTag;
using SoulSmith.Battle.Effects;
using System.Text.Json.Serialization;
using SoulSmith.Asset;

namespace SoulSmith.Emotion
{

    [JsonConverter(typeof(EmotionJsonConverter))]
    public class Emotion : IDisposable
    {
        private static Dictionary<EmotionTag.EmotionTag, Emotion> _emotionCache = new Dictionary<EmotionTag.EmotionTag, Emotion>();
        public EmotionTag.EmotionTag EmotionTag { get; }
        public Color Color { get; }
        public string FriendlyName { get; }
        public string FormKey { get; }
        public ReadOnlyCollection<IEffect> BattleEntryEffects { get; }

        public Emotion(EmotionTag.EmotionTag emotionTag, string friendlyName, string formKey, Color color, IEnumerable<IEffect> battleEntryEffects)
        {
            Color = color;
            FriendlyName = friendlyName;
            FormKey = formKey;
            BattleEntryEffects = battleEntryEffects.ToList().AsReadOnly();
            EmotionTag = emotionTag;
        }

        public void Dispose()
        {
            if (BattleEntryEffects != null)
            {
                foreach (IEffect effect in BattleEntryEffects)
                {
                    effect.Dispose();
                }
            }
        }

        public static Emotion GetEmotion(EmotionTag.EmotionTag emotionTag)
        {
            if (_emotionCache.ContainsKey(emotionTag))
            {
                return _emotionCache[emotionTag];
            }
            
            Emotion emotion = AssetManager.Instance.GetEmotion<Emotion>(GetEmotionKey(emotionTag));

            if (emotion == null)
            {
                return null;
            }

            _emotionCache[emotionTag] = emotion;

            return emotion;
        }

        private static string GetEmotionKey(EmotionTag.EmotionTag emotionTag)
        {
            switch(emotionTag)
            {
                case SoulSmith.EmotionTag.EmotionTag.Joy:
                    return "Emotions/Single/Joy";
                case SoulSmith.EmotionTag.EmotionTag.Typeless:
                    return "Emotions/Typeless";
                default:
                    throw new KeyNotFoundException($"Emotion with tag {emotionTag} does not have a predefined key.");
            }
        }
    }
}