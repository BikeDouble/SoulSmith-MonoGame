using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SoulSmith.Battle.Effect;

namespace SoulSmith.Effect
{
    public class IEffectJsonConverter : JsonConverter<IEffect>
    {
        public override IEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            if (reader.GetString() != "Type") throw new JsonException("Expected type of effect");

            reader.Read();

            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected name of Effect type");

            string effectType = reader.GetString();

            reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");

            switch (effectType)
            {
                case "HitDamageFormula":
                    return JsonSerializer.Deserialize<HitDamageFormulaEffect>(ref reader, options);
                default:
                    throw new JsonException($"Unexpected type {effectType}. Type is either misspelled or does not exist.");
            }
        }

        public override void Write(Utf8JsonWriter writer, IEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
