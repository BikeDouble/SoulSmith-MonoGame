using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers
{
    [JsonConverter(typeof(ModifierAlignmentJsonConverter))]
    public enum ModifierAlignment
    {
        Null,
        Neutral,
        Buff,
        Debuff
    }

    public class ModifierAlignmentJsonConverter : System.Text.Json.Serialization.JsonConverter<ModifierAlignment>
    {
        public override ModifierAlignment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected to decode a string");
            string text = reader.GetString() ?? throw new JsonException("String cannot be null");

            string textLower = text.ToLower();

            switch (textLower)
            {
                case "buff":
                    return ModifierAlignment.Buff;
                case "debuff":
                    return ModifierAlignment.Debuff;
                case "neutral":
                    return ModifierAlignment.Neutral;
                default:
                    throw new JsonException($"Unknown ModifierAlignment value: {text}. Expected 'buff', 'debuff', or 'neutral'.");
            }
        }

        public override void Write(Utf8JsonWriter writer, ModifierAlignment value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Writing ModifierAlignment to JSON is not implemented yet.");
        }
    }
}
