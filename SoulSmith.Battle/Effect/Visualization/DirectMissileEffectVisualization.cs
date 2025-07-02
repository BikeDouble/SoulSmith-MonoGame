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

        public DirectMissileEffectVisualization(string missileResourceKey,
            float lifespan,
            float effectActivationTimer = -1,
            float delay = 0f) : base(lifespan, effectActivationTimer, delay)
        {
            IReadOnlyTrackedAsset<DrawableResource_Texture2D> missileAsset = AssetManager.Instance.GetTexture2D<DrawableResource_Texture2D>(missileResourceKey);
            _missile = new CanvasObject(null, missileAsset);
            AddChild(_missile);
        }

        public DirectMissileEffectVisualization(IReadOnlyTrackedAsset<IDrawableResource> missileResource,
            float lifespan,
            float effectActivationTimer = -1,
            float delay = 0f) : base(lifespan, effectActivationTimer, delay)
        {
            _missile = new CanvasObject(null, missileResource);
            AddChild(_missile);
        }

        public override void BeginVisualization(IReadOnlyUnit sender, IReadOnlyUnit target, float delay = 0)
        {
            base.BeginVisualization(sender, target, delay);

            _startPoint = Sender.HitZone.GetRandomGlobalPoint(Sender.GetGlobalPosition());

            _missile.Set(_startPoint);

            _endPoint = Target.HitZone.GetRandomGlobalPoint(Target.GetGlobalPosition());
        }

        protected override void EnabledProcess(double delta)
        {
            base.EnabledProcess(delta);

            double interpolant = Math.Clamp(ElapsedLifespan / TotalLifespan, 0, 1);
            Vector2 difference = _endPoint - _startPoint;
            Vector2 desiredPosition = _startPoint + (float)interpolant * difference;

            _missile.Set(desiredPosition);
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
