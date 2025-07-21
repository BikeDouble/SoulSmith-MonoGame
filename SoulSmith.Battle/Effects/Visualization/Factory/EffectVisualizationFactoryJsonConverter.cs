using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Visualization.Factory
{
    public class EffectVisualizationFactoryJsonConverter : JsonConverter<EffectVisualizationFactory>
    {
        public override EffectVisualizationFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            if (reader.GetString() != "Type") throw new JsonException("Expected type of EffectVisualization");

            reader.Read();

            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected name of EffectVisualization type");

            string visType = reader.GetString();

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            if (reader.GetString() != "Visualization") throw new JsonException("Expected visualization");

            reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");

            EffectVisualizationFactory value = null;

            switch (visType)
            {
                case "DirectMissile":
                    value = JsonSerializer.Deserialize<DirectMissileEffectVisualizationFactory>(ref reader, options);
                    reader.Read();
                    break;
                case "GrowAndFadeOnTarget":
                    value = JsonSerializer.Deserialize<GrowAndFadeOnTargetEffectVisualizationFactory>(ref reader, options);
                    reader.Read();
                    break;
                default:
                    throw new JsonException($"Unexpected type {visType}. Type is either misspelled or does not exist.");
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, EffectVisualizationFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
