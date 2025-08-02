using Microsoft.Xna.Framework;
using SoulSmith.Core;
using SoulSmith.Vector;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Shapes
{
    [JsonConverter(typeof(CircleJsonConverter))]
    public class Circle : IZone
    {
        private float _radius;
        private Vector2 _offset;

        public Circle(float radius)
        {
            _radius = radius;
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation)
        {
            Vector2 localPoint = Position.InverseTransformPoint(point, transformation);

            return ContainsLocal(localPoint);
        }

        public bool ContainsLocal(Vector2 point)
        {
            Vector2 newPoint = point - _offset;

            return Math.Pow(newPoint.X, 2) + Math.Pow(newPoint.Y, 2) <= Math.Pow(_radius, 2); 
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition transformation)
        {
            Vector2 localPoint = GetRandomLocalPoint();
            return Position.TransformPoint(localPoint, transformation);
        }

        public Vector2 GetRandomLocalPoint()
        {
            float angle = Rand.RandFloat() * 360;
            float distanceFromRadius = Rand.RandFloat() * _radius;

            float x = (float)(Math.Cos(angle) * distanceFromRadius);
            float y = (float)(Math.Sin(angle) * distanceFromRadius);

            Vector2 randomPoint = new Vector2(x, y);

            randomPoint += _offset;

            return randomPoint;
        }

        public float GetAreaLocal()
        {
            return (float)(Math.PI * Math.Pow(_radius, 2));
        }

        public float GetHeightLocal()
        {
            return _radius * 2;
        }

        public float GetWidthLocal()
        {
            return _radius * 2;
        }
    }

    public class CircleJsonConverter : System.Text.Json.Serialization.JsonConverter<Circle>
    {
        public override Circle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of object");
            reader.Read();

            float? radius = null;
            Vector2 offset = Vector2.Zero;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
                reader.Read();

                switch (propertyName.ToLower())
                {
                    case "radius":
                        radius = reader.GetSingle();
                        reader.Read();
                        break;
                    case "diameter":
                        radius = reader.GetSingle() / 2;
                        reader.Read();
                        break;
                    case "offset":
                    case "position":
                        Vector2JsonConverter vector2JsonConverter = new Vector2JsonConverter();
                        offset = vector2JsonConverter.Read(ref reader, typeof(Vector2), options);
                        reader.Read(); 
                        break;
                    default:
                        throw new JsonException($"Unknown property: {propertyName}");
                }
                reader.Read();
            }

            if (!radius.HasValue)
            {
                throw new JsonException("Radius not defined");
            }

            return new Circle(radius.Value);
        }

        public override void Write(Utf8JsonWriter writer, Circle value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Serialization not implemented for Circle");
        }
    }
}
