using SoulSmith.Asset;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SoulSmith.Battle.Effect.Visualization
{
    [JsonConverter(typeof(DirectMissileEffectVisualizationJsonConverter))]
    public class DirectMissileEffectVisualization : EffectVisualization
    {
        // Children
        private CanvasObject _missile;
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private string _missileResourceKey;

        public DirectMissileEffectVisualization(string missileResourceKey,
            float lifespan,
            float effectActivationTimer = -1,
            float baseDelay = 0f) : base(lifespan, effectActivationTimer, baseDelay)
        {
            _missileResourceKey = missileResourceKey;
            IAssetWrapper<Texture2DResource> missileAsset = AssetManager.Instance.GetTexture2DResource<Texture2DResource>(missileResourceKey);
            _missile = new CanvasObject(null, missileAsset);
            AddChild(_missile);
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0)
        {
            base.BeginVisualization(sender, target, delay);

            if (Sender.FireZone != null)
            {
                _startPoint = Sender.FireZone.GetRandomGlobalPoint(Sender.GetGlobalPosition());
            }
            else
            {
                _startPoint = Sender.GetGlobalPosition().Coordinates;
            }

            _missile.Set(_startPoint);

            if (Target.HitZone != null)
            {
                _endPoint = Target.HitZone.GetRandomGlobalPoint(Target.GetGlobalPosition());
            }
            else
            {
                _endPoint = Target.GetGlobalPosition().Coordinates;
            }
        }

        protected override void EnabledProcess(double delta)
        {
            base.EnabledProcess(delta);

            double interpolant = Math.Clamp(ElapsedLifespan / TotalLifespan, 0, 1);
            Vector2 difference = _endPoint - _startPoint;
            Vector2 desiredPosition = _startPoint + (float)interpolant * difference;

            _missile.Set(desiredPosition);
        }

        public override EffectVisualization CloneVisualization()
        {
            return new DirectMissileEffectVisualization(_missileResourceKey, TotalLifespan, EffectActivationTimer, BaseDelay);
        }
    }

    public class DirectMissileEffectVisualizationJsonConverter : JsonConverter<DirectMissileEffectVisualization>
    {
        public override DirectMissileEffectVisualization Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            float flightTime = 0f;
            float delay = 0f;
            string missileTexturePath = string.Empty;

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
                        missileTexturePath = reader.GetString();
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new DirectMissileEffectVisualization(missileTexturePath, flightTime, flightTime, delay);
        }

        public override void Write(Utf8JsonWriter writer, DirectMissileEffectVisualization value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
