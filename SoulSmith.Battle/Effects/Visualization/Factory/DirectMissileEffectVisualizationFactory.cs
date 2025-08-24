using Microsoft.Xna.Framework;
using SoulSmith.Battle.Emotions;
using SoulSmith.Battle.Modifiers.Payload;
using SoulSmith.Drawing;
using SoulSmith.EmotionTags;
using SoulSmith.Templates;
using SoulSmith.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects.Visualization.Factory
{
    [JsonConverter(typeof(DirectMissileEffectVisualizationFactoryJsonConverter))]
    public class DirectMissileEffectVisualizationFactory : EffectVisualizationFactory
    {
        private string _missileResourceKey;
        private Vector2? _missileSizeInPixels;

        public DirectMissileEffectVisualizationFactory(string missileResourceKey,
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
        public readonly static Vector2 STANDARDEMOTIONMISSILESIZE = new Vector2(100, 100);

        public override DirectMissileEffectVisualizationFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            float flightTime = 0f;
            float delay = 0f;
            float sizeMod = 1f;
            string missileResourceKey = null;
            Vector2? missileSizeInPixels = null;
            EmotionTag? emotionTag = null;

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
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        missileResourceKey = reader.GetString();
                        reader.Read();
                        break;
                    case "MissileSizeInPixels":
                        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
                        Vector2JsonConverter converter = new Vector2JsonConverter();
                        missileSizeInPixels = converter.Read(ref reader, typeof(Vector2), options);
                        reader.Read();
                        break;
                    case "EmotionTag":
                    case "Emotion":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        emotionTag = (EmotionTag)reader.GetInt32();
                        reader.Read();
                        break;
                    case "SizeMod":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        sizeMod = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (emotionTag.HasValue)
            {
                UnitTemplate unitTemplate = Emotion.GetEmotion(emotionTag.Value).UnitTemplate;

                missileResourceKey = unitTemplate.SpriteName;
                missileSizeInPixels = unitTemplate.SpriteSizeMod * STANDARDEMOTIONMISSILESIZE * sizeMod;
            }

            return new DirectMissileEffectVisualizationFactory(missileResourceKey, flightTime, flightTime, delay, missileSizeInPixels);
        }

        public override void Write(Utf8JsonWriter writer, DirectMissileEffectVisualizationFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
