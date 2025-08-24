using Microsoft.Xna.Framework;
using SoulSmith.Asset;
using SoulSmith.Battle.Effects;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Emotions
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
        public ReadOnlyCollection<EmotionTag.EmotionTag> BasicEmotions { get; }

        public Emotion(EmotionTag.EmotionTag emotionTag, string friendlyName, string formKey, Color color, IEnumerable<IEffect> battleEntryEffects)
        {
            Color = color;
            FriendlyName = friendlyName;
            FormKey = formKey;
            BattleEntryEffects = battleEntryEffects.ToList().AsReadOnly();
            EmotionTag = emotionTag;
            BasicEmotions = GetBaseEmotionTags(emotionTag).AsReadOnly();
        }

        public static List<EmotionTag.EmotionTag> GetBaseEmotionTags(EmotionTag.EmotionTag value)
        {
            List<EmotionTag.EmotionTag> baseEmotionTags = new List<EmotionTag.EmotionTag>();

            int valueAsInt = (int)value;

            for (int bit = 0; bit < 8; bit++)
            {
                int mask = 1 << bit;
                if ((valueAsInt & mask) != 0)
                    baseEmotionTags.Add((EmotionTag.EmotionTag)mask);
            }

            return baseEmotionTags;
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

        //public static UnitTemplate GetFormTemplate(EmotionTag.EmotionTag emotionTag)
        //{
        //    Emotion emotion = GetEmotion(emotionTag);

        //    emotion.UnitTemplateKey
        //}

        private static string GetEmotionKey(EmotionTag.EmotionTag emotionTag)
        {
            switch(emotionTag)
            {
                case SoulSmith.EmotionTag.EmotionTag.Typeless:
                    return "Emotions/Typeless";
                case SoulSmith.EmotionTag.EmotionTag.Joy:
                    return "Emotions/Single/Joy";
                case SoulSmith.EmotionTag.EmotionTag.Wrath:
                    return "Emotions/Single/Anger";
                case SoulSmith.EmotionTag.EmotionTag.Exultation:
                    return "Emotions/Double/JoyAnger";
                default:
                    throw new KeyNotFoundException($"Emotion with tag {emotionTag} does not have a predefined key.");
            }
        }
    }
}