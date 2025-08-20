using Microsoft.Xna.Framework;
using SoulSmith.Drawing;
using SoulSmith.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects.Visualization.Factory
{
    [JsonConverter(typeof(ShockwaveVisualizationFactoryJsonConverter))]
    public class ShockwaveVisualizationFactory : EffectVisualizationFactory
    {
        private Color _tint;
        private string _particleResourceKey;
        private int _particleCount;
        private int _particleToExecute;
        private float _delayBetweenParticles;
        private float _fadeTime;


        public ShockwaveVisualizationFactory(string particleResourceKey,
            int particleCount,
            int particleToExecute,
            float delayBetweenParticles,
            float effectActivationTimer,
            float fadeTime,
            Color tint,
            float delay) : base(ShockwaveVisualization.CalculateLifespan(particleCount, delayBetweenParticles, effectActivationTimer, particleToExecute, fadeTime), effectActivationTimer, delay)
        {
            _particleCount = particleCount;
            _particleToExecute = particleToExecute;
            _delayBetweenParticles = delayBetweenParticles;
            _fadeTime = fadeTime;
            _particleResourceKey = particleResourceKey;
            _tint = tint;
        }

        public override EffectVisualization CreateVisualization()
        {
            return new ShockwaveVisualization(_particleResourceKey, _particleCount, _particleToExecute, _delayBetweenParticles, EffectActivationTimer, _fadeTime, _tint, Delay);
        }
    }

    public class ShockwaveVisualizationFactoryJsonConverter : JsonConverter<ShockwaveVisualizationFactory>
    {
        public override ShockwaveVisualizationFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            float effectActivationDelay = 1f;
            float delayBetweenParticles = 0.3f;
            float fadeTime = 0.15f;
            int particleCount = 1;
            int particleToExecute = 1;
            float delay = 0f;
            string particleResourceKey = null;
            Color tint = Color.White;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "EffectActivationDelay":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        effectActivationDelay = reader.GetSingle();
                        reader.Read();
                        break;
                    case "Delay":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        delay = (float)reader.GetDouble();
                        reader.Read();
                        break;
                    case "DelayBetweenParticles":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        delayBetweenParticles = reader.GetSingle();
                        reader.Read();
                        break;
                    case "ParticleCount":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        particleCount = reader.GetInt32();
                        reader.Read();
                        break;
                    case "ParticleToExecute":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        particleToExecute = reader.GetInt32();
                        reader.Read();
                        break;
                    case "FadeTime":
                    case "ParticleFadeTime":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        fadeTime = reader.GetSingle();
                        reader.Read();
                        break;
                    case "ParticleSprite":
                    case "Sprite":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        particleResourceKey = reader.GetString();
                        reader.Read();
                        break;
                    case "Tint":
                    case "Color":
                        JsonConverter<Color> colorConverter = new ColorJsonConverter();
                        tint = colorConverter.Read(ref reader, typeof(Color), options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (particleResourceKey == null) throw new JsonException("ParticleSprite is required");

            return new ShockwaveVisualizationFactory(particleResourceKey, particleCount, particleToExecute, delayBetweenParticles, effectActivationDelay, fadeTime, tint, delay);
        }

        public override void Write(Utf8JsonWriter writer, ShockwaveVisualizationFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
