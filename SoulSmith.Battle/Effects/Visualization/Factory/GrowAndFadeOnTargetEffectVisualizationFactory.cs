using Microsoft.Xna.Framework;
using SoulSmith.Drawing;
using SoulSmith.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects.Visualization.Factory
{
    [JsonConverter(typeof(GrowAndFadeOnTargetEffectVisualizationFactoryJsonConverter))]
    public class GrowAndFadeOnTargetEffectVisualizationFactory : EffectVisualizationFactory
    {
        private Vector2 _startSize;
        private Vector2 _endSize;
        private DrawableResourceKey _particleResourceKey;

        public GrowAndFadeOnTargetEffectVisualizationFactory(DrawableResourceKey particleResourceKey,
            Vector2 startSize,
            Vector2 endSize,
            float lifespan,
            float effectActivationTimer = -1,
            float delay = 0f) : base(lifespan, effectActivationTimer, delay)
        {
            _particleResourceKey = particleResourceKey;
            _startSize = startSize;
            _endSize = endSize;
        }

        public override EffectVisualization CreateVisualization()
        {
            return new GrowAndFadeOnTargetEffectVisualization(_particleResourceKey, _startSize, _endSize, Lifespan, EffectActivationTimer, Delay);
        }
    }

    public class GrowAndFadeOnTargetEffectVisualizationFactoryJsonConverter : JsonConverter<GrowAndFadeOnTargetEffectVisualizationFactory>
    {
        public override GrowAndFadeOnTargetEffectVisualizationFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            float effectActivationDelay = 0f;
            float lifespan = 0f;
            float delay = 0f;
            DrawableResourceKey particleResourceKey = null;
            Vector2 startSize = Vector2.Zero;
            Vector2? endSize = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "EffectActivationDelay":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        effectActivationDelay = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "Delay":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        delay = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "Lifespan":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        lifespan = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "ParticleSprite":
                    case "Sprite":
                        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
                        particleResourceKey = JsonSerializer.Deserialize<DrawableResourceKey>(ref reader, options);
                        reader.Read();
                        break;
                    case "StartSizeInPixels":
                    case "StartSize":
                        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
                        Vector2JsonConverter converter = new Vector2JsonConverter();
                        startSize = converter.Read(ref reader, typeof(Vector2), options);
                        reader.Read();
                        break;
                    case "EndSizeInPixels":
                    case "EndSize":
                        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
                        converter = new Vector2JsonConverter();
                        endSize = converter.Read(ref reader, typeof(Vector2), options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (particleResourceKey == null) throw new JsonException("ParticleSprite is required");
            if (endSize == null) throw new JsonException("EndSize is required");
            if (lifespan <= 0) throw new JsonException("Lifespan must be greater than 0");

            return new GrowAndFadeOnTargetEffectVisualizationFactory(particleResourceKey, startSize, endSize.Value, lifespan, effectActivationDelay, delay);
        }

        public override void Write(Utf8JsonWriter writer, GrowAndFadeOnTargetEffectVisualizationFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
