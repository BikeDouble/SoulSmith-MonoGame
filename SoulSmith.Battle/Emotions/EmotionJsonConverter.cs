using Microsoft.Xna.Framework;
using SoulSmith.Battle.Effects;
using SoulSmith.Drawing;
using System.Text.Json;

namespace SoulSmith.Battle.Emotions
{
    public class EmotionJsonConverter : System.Text.Json.Serialization.JsonConverter<Emotion>
    {
        public override Emotion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            string friendlyName = string.Empty;
            string formKey = string.Empty;
            Color color = Color.White;
            EmotionTags.EmotionTag emotionTag = EmotionTags.EmotionTag.Typeless;
            IEffect[] battleEntryEffects = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "FriendlyName":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        friendlyName = reader.GetString();
                        reader.Read();
                        break;
                    case "FormKey":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        formKey = reader.GetString();
                        reader.Read();
                        break;
                    case "Color":
                        ColorJsonConverter converter = new ColorJsonConverter();
                        color = converter.Read(ref reader, typeof(Color), options);
                        reader.Read();
                        break;
                    case "EmotionTags":
                    case "Tag":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        int emotionTagValue = reader.GetInt32();
                        emotionTag = (EmotionTags.EmotionTag)emotionTagValue;
                        reader.Read();
                        break;
                    case "BattleEntryEffects":
                    case "CombatEntryEffects":
                        if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected start of an array");
                        battleEntryEffects = JsonSerializer.Deserialize<IEffect[]>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        reader.Read();
                        break;
                }
            }

            return new Emotion(
                emotionTag,
                friendlyName,
                formKey,
                color,
                battleEntryEffects
            );
        }
        public override void Write(Utf8JsonWriter writer, Emotion value, JsonSerializerOptions options)
        {
            // Implement serialization logic here
            throw new NotImplementedException("Serialization not implemented yet.");
        }
    }
}
