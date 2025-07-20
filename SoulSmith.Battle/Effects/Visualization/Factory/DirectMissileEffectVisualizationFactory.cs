using Microsoft.Xna.Framework;
using SoulSmith.Drawing;
using SoulSmith.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects.Visualization.Factory
{
    [JsonConverter(typeof(DirectMissileEffectVisualizationFactoryJsonConverter))]
    public class DirectMissileEffectVisualizationFactory : EffectVisualizationFactory
    {
        private DrawableResourceKey _missileResourceKey;
        private Vector2? _missileSizeInPixels;

        public DirectMissileEffectVisualizationFactory(DrawableResourceKey missileResourceKey,
            float lifespan,
            float effectActivationTimer = -1,
            float baseDelay = 0f,
            Vector2? missileSizeInPixels = null) : base(lifespan, effectActivationTimer, baseDelay)
        {
            _missileResourceKey = missileResourceKey;
            _missileSizeInPixels = missileSizeInPixels;
        }

        public override EffectVisualization CreateVisualization()
        {
            return new DirectMissileEffectVisualization(_missileResourceKey, Lifespan, EffectActivationTimer, Delay, _missileSizeInPixels);
        }
    }

    public class DirectMissileEffectVisualizationFactoryJsonConverter : JsonConverter<DirectMissileEffectVisualizationFactory>
    {
        public override DirectMissileEffectVisualizationFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            float flightTime = 0f;
            float delay = 0f;
            DrawableResourceKey missileResourceKey = null;
            Vector2? missileSizeInPixels = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "FlightTime":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        flightTime = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "Delay":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        delay = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "MissileSprite":
                        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
                        missileResourceKey = JsonSerializer.Deserialize<DrawableResourceKey>(ref reader, options);
                        reader.Read();
                        break;
                    case "MissileSizeInPixels":
                        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
                        Vector2JsonConverter converter = new Vector2JsonConverter();
                        missileSizeInPixels = converter.Read(ref reader, typeof(Vector2), options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new DirectMissileEffectVisualizationFactory(missileResourceKey, flightTime, flightTime, delay, missileSizeInPixels);
        }

        public override void Write(Utf8JsonWriter writer, DirectMissileEffectVisualizationFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
