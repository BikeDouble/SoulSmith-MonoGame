using SoulSmith.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effect
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

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            if (reader.GetString() != "Effect") throw new JsonException("Expected effect");

            reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");

            IEffect value = null;

            switch (effectType)
            {
                case "HitDamageFormula":
                    value = JsonSerializer.Deserialize<HitDamageFormulaEffect>(ref reader, options);
                    reader.Read();
                    break;
                case "SimpleStaticRoundDurationStatModifier":
                    value = JsonSerializer.Deserialize<SimpleStaticRoundDurationStatModifierEffect>(ref reader, options);
                    reader.Read();
                    break;
                default:
                    throw new JsonException($"Unexpected type {effectType}. Type is either misspelled or does not exist.");
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, IEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
