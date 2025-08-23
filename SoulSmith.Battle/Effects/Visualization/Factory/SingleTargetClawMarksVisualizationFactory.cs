using Microsoft.Xna.Framework;
using SoulSmith.Drawing;
using SoulSmith.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects.Visualization.Factory
{
    [JsonConverter(typeof(SingleTargetClawMarksVisualizationFactoryJsonConverter))]
    public class SingleTargetClawMarksVisualizationFactory : EffectVisualizationFactory
    {
        private Vector2 _size;
        private Color _color;
        private string _particleResourceKey;
        private float _slashTime;
        private float _lingerTime;
        private float _fadeTime;

        public SingleTargetClawMarksVisualizationFactory(string particleResourceKey,
            Vector2 size,
            Color color,
            float slashTime,
            float lingerTime,
            float fadeTime,
            float delay = 0f) : base(slashTime + lingerTime + fadeTime, slashTime / 2, delay)
        {
            _particleResourceKey = particleResourceKey;
            _size = size;
            _color = color;
            _slashTime = slashTime;
            _lingerTime = lingerTime;
            _fadeTime = fadeTime;
        }

        public override EffectVisualization CreateVisualization()
        {
            return new SingleTargetClawMarksVisualization(_particleResourceKey, _size, _color, _slashTime, _lingerTime, _fadeTime, Lifespan, EffectActivationTimer, Delay);
        }
    }

    public class SingleTargetClawMarksVisualizationFactoryJsonConverter : JsonConverter<SingleTargetClawMarksVisualizationFactory>
    {
        public override SingleTargetClawMarksVisualizationFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            float slashTime = 0f;
            float lingerTime = 0f;
            float fadeTime = 0f;
            float delay = 0f;
            string particleResourceKey = null;
            Vector2? size = null;
            Color color = Color.White;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Delay":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        delay = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "SlashTime":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        slashTime = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "LingerTime":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        lingerTime = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "FadeTime":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        fadeTime = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "ParticleSprite":
                    case "Sprite":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        particleResourceKey = reader.GetString();
                        reader.Read();
                        break;
                    case "Size":
                        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");
                        Vector2JsonConverter converter = new Vector2JsonConverter();
                        size = converter.Read(ref reader, typeof(Vector2), options);
                        reader.Read();
                        break;
                    case "Color":
                        if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected start of array");
                        ColorJsonConverter colorConverter = new ColorJsonConverter();
                        color = colorConverter.Read(ref reader, typeof(Color), options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (particleResourceKey == null) throw new JsonException("ParticleSprite is required");
            if (size == null) throw new JsonException("Size is required");
            if (slashTime + fadeTime + lingerTime <= 0) throw new JsonException("Lifespan must be greater than 0");

            return new SingleTargetClawMarksVisualizationFactory(particleResourceKey, size.Value, color, slashTime, lingerTime, fadeTime, delay);
        }

        public override void Write(Utf8JsonWriter writer, SingleTargetClawMarksVisualizationFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
