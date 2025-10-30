using Microsoft.Xna.Framework;
using SoulSmith.Asset;
using SoulSmith.Battle.Effects;
using SoulSmith.EmotionTags;
using SoulSmith.Templates;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Emotions
{

    [JsonConverter(typeof(EmotionJsonConverter))]
    public class Emotion : IDisposable
    {
        private static Dictionary<EmotionTags.EmotionTag, Emotion> _emotionCache = new Dictionary<EmotionTags.EmotionTag, Emotion>();

        private IAssetWrapper<UnitTemplate> _unitTemplateWrapper;
        public EmotionTags.EmotionTag EmotionTag { get; }
        public Color Color { get; }
        public string FriendlyName { get; }
        public string FormKey { get; }
        public ReadOnlyCollection<IEffect> BattleEntryEffects { get; }
        public ReadOnlyCollection<EmotionTags.EmotionTag> BasicEmotions { get; }
        public UnitTemplate UnitTemplate { get { return _unitTemplateWrapper.Value; } }

        public Emotion(EmotionTags.EmotionTag emotionTag, string friendlyName, string formKey, Color color, IEnumerable<IEffect> battleEntryEffects)
        {
            Color = color;
            FriendlyName = friendlyName;
            FormKey = formKey;
            BattleEntryEffects = battleEntryEffects.ToList().AsReadOnly();
            EmotionTag = emotionTag;
            BasicEmotions = GetBaseEmotionTags(emotionTag).AsReadOnly();
            _unitTemplateWrapper = AssetManager.Instance.GetUnitTemplate<UnitTemplate>(FormKey);
        }

        public static List<EmotionTags.EmotionTag> GetBaseEmotionTags(EmotionTags.EmotionTag value)
        {
            List<EmotionTags.EmotionTag> baseEmotionTags = new List<EmotionTags.EmotionTag>();

            int valueAsInt = (int)value;

            for (int bit = 0; bit < 8; bit++)
            {
                int mask = 1 << bit;
                if ((valueAsInt & mask) != 0)
                    baseEmotionTags.Add((EmotionTags.EmotionTag)mask);
            }

            return baseEmotionTags;
        }

        public static EmotionTag CombineTags(EmotionTag a, EmotionTag b)
        {
            return a | b; //TODO test
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

        public static Emotion GetEmotion(EmotionTags.EmotionTag emotionTag)
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

        private static string GetEmotionKey(EmotionTags.EmotionTag emotionTag)
        {
            switch(emotionTag)
            {
                case SoulSmith.EmotionTags.EmotionTag.Typeless:
                    return "Emotions/Typeless";
                case SoulSmith.EmotionTags.EmotionTag.Joy:
                    return "Emotions/Single/Joy";
                case SoulSmith.EmotionTags.EmotionTag.Wrath:
                    return "Emotions/Single/Anger";
                case SoulSmith.EmotionTags.EmotionTag.Pride:
                    return "Emotions/Double/JoyAnger";
                default:
                    throw new KeyNotFoundException($"Emotion with tag {emotionTag} does not have a predefined key.");
            }
        }
    }
}