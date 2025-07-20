using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Drawing;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace SoulSmith.Emotion
{
    public class EmotionJsonConverter : System.Text.Json.Serialization.JsonConverter<Emotion>
    {
        public override Emotion Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            string friendlyName = string.Empty;
            string formKey = string.Empty;
            Color color = Color.White;
            EmotionTag.EmotionTag emotionTag = EmotionTag.EmotionTag.Typeless;
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
                    case "EmotionTag":
                    case "Tag":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        int emotionTagValue = reader.GetInt32();
                        emotionTag = (EmotionTag.EmotionTag)emotionTagValue;
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
        public override void Write(System.Text.Json.Utf8JsonWriter writer, Emotion value, System.Text.Json.JsonSerializerOptions options)
        {
            // Implement serialization logic here
            throw new NotImplementedException("Serialization not implemented yet.");
        }
    }
}
