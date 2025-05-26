using Microsoft.Xna.Framework;
using SoulSmith.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Shapes
{
    [JsonConverter(typeof(PolygonJsonConverter))]
    public class Polygon : IZone, ITransformable, IDeepCloneable
    {
        private List<Vector2> _vertices;

        public Polygon(IEnumerable<Vector2> vertices)
        {
            _vertices = new List<Vector2>(vertices);
        }

        public bool Contains(Vector2 point)
        {
            if (_vertices.Count < 3) return false;

            return CountRayCastIntersections(point) % 2 == 1;
        }

        private int CountRayCastIntersections(Vector2 point)
        {
            int intersections = 0;

            for (int i = 0; i < _vertices.Count; i ++)
            {
                Vector2 vertex1 = _vertices[i];

                Vector2 vertex2 = (i == _vertices.Count - 1) ? _vertices[0] : _vertices[i + 1];

                if (vertex1.Y != vertex2.Y)
                {
                    if ((vertex1.Y > point.Y) != (vertex2.Y > point.Y))
                    {
                        float intersectX = CalculateIntersectX(vertex1, vertex2, point.Y);

                        if (intersectX > point.X) intersections++;
                    }
                }
            }
            return intersections;
        }

        private static float CalculateIntersectX(Vector2 a, Vector2 b, float lineY)
        {
            float numerator = (lineY - a.Y) * (b.X - a.X);
            float denominator = (b.Y - a.Y);

            return a.X + numerator / denominator;
        }

        public static Polygon RegularPolygon(float radius, int sides)
        {
            if (sides < 3)
                return null;

            if (radius < 0)
                return null;

            float rotationPer = Position.MAXROTATION / sides;

            List<Vector2> vertices = new List<Vector2>();

            for (int i = 0; i < sides; i++)
            {
                vertices.Add(Position.RotatePointAroundPoint(new Vector2(radius, 0), Vector2.Zero, i * rotationPer));
            }

            return new Polygon(vertices);
        }

        public object DeepClone()
        {
            return new Polygon(_vertices);
        }

        public void Set(Position position)
        {

        }

        public void Set(Vector2 coordinates)
        {

        }

        public void Transform(IReadOnlyPosition transformation)
        {
            Scale(transformation.ScaleVector);
            Translate(transformation.Coordinates);
            Rotate(transformation.Rotation);
        }

        public void Translate(Vector2 translation)
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = _vertices[i] + translation;
            }
        }

        public void Scale(Vector2 scale)
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = _vertices[i] * scale;
            }
        }

        public void Rotate(float rotation, Vector2? origin = null)
        {
            Vector2 originVal = origin ?? Vector2.Zero;

            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = Position.RotatePointAroundPoint(originVal, _vertices[i], rotation);
            }
        }
    }

    internal class PolygonJsonConverter : System.Text.Json.Serialization.JsonConverter<Polygon>
    {
        public override Polygon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of object");
            reader.Read();
            string text = reader.GetString() ?? throw new JsonException("String cannot be null");

            bool foundVertices = false;
            bool foundRadius = false;
            Polygon polygon = null;
            int sides = 0;
            float radius = 0;
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
                reader.Read();

                switch (propertyName)
                {
                    case "Vertices":
                        if (!foundVertices)
                        {
                            var serializationOptionsWithConverter = new JsonSerializerOptions();
                            serializationOptionsWithConverter.Converters.Add(new Vector2ArrayConverter());
                            Vector2[] vertices = JsonSerializer.Deserialize<Vector2[]>(ref reader, serializationOptionsWithConverter);

                            polygon = new Polygon(vertices);
                            foundVertices = true;
                        }
                        break;
                    case "Radius":
                        if (!foundRadius)
                        {
                            radius = reader.GetSingle();
                            foundRadius = true;
                        }
                        break;
                    case "Sides":
                        sides = reader.GetInt32();
                        break;
                    default:
                        throw new JsonException($"Unknown property: {propertyName}");
                }
                reader.Read();
            }

            if (polygon is null)
            {
                if (foundRadius && (sides > 0) && (radius > 0))
                {
                    polygon = Polygon.RegularPolygon(radius, sides);
                }
                else
                {
                    throw new JsonException("Vertices not defined");
                }
            }

            return polygon;
        }

        public override void Write(Utf8JsonWriter writer, Polygon value, JsonSerializerOptions options)
        {
            //TODO
        }
    }

    internal class ColorConverter : JsonConverter<Color> //TODO remove
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected StartArray token");

            float[] floats = new float[4];

            int i = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType != JsonTokenType.Number)
                    throw new JsonException("Expected Number");

                floats[i] = reader.GetSingle();
            }

            return new Color(floats[0], floats[1], floats[2], floats[3]);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            return; //TODO
        }
    }

    internal class Vector2ArrayConverter : JsonConverter<Vector2[]> //TODO remove
    {
        public override Vector2[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var vertices = new List<Vector2>();

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected StartArray token");

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException("Expected StartObject for Vector2");

                float x = 0, y = 0;

                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType != JsonTokenType.PropertyName)
                        continue;

                    string propName = reader.GetString();
                    reader.Read();

                    switch (propName)
                    {
                        case "X":
                        case "x":
                            x = reader.GetSingle();
                            break;
                        case "Y":
                        case "y":
                            y = reader.GetSingle();
                            break;
                    }
                }

                vertices.Add(new Vector2(x, y));
            }

            return vertices.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, Vector2[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var v in value)
            {
                writer.WriteStartObject();
                writer.WriteNumber("X", v.X);
                writer.WriteNumber("Y", v.Y);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
