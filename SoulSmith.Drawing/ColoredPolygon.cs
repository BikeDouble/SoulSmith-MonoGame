using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MonoGame.Extended.Serialization.Json;
using System.Text.Json;

namespace SoulSmith.Drawing
{
    [JsonConverter(typeof(ColoredPolygonJsonConverter))]
    public class ColoredPolygon
    {
        public Polygon Polygon;
        public Color Color;
        public bool Filled;
        public int LineThickness;

        public ColoredPolygon(ColoredPolygon other)
        {
            Polygon = DeepCopy(other.Polygon);
            Color = other.Color;
        }

        public ColoredPolygon(Polygon polygon, int[] color, int lineThickness, bool filled)
        {
            if ((color == null) || (color?.Length < 4))
            {
                Color = Color.White;
            }
            else
            {
                Color = new Color(color[0], color[1], color[2], color[3]);
            }

            Polygon = DeepCopy(polygon);
            Filled = filled;
            LineThickness = lineThickness;
        }

        public ColoredPolygon(Polygon polygon, Color color, int lineThickness, bool filled)
        {
            Color = color;
            Polygon = DeepCopy(polygon);
            Filled = filled;
            LineThickness = lineThickness;
        }

        public static Polygon RegularPolygon(float radius, int sides)
        {
            if (sides < 3)
                return null;

            if (radius < 0)
                return null;

            float rotationPer = CanvasPosition.MAXROTATION / sides;

            List<Vector2> vertices = new List<Vector2>();

            for (int i = 0; i < sides; i++)
            {
                vertices.Add(CanvasPosition.RotatePointAroundPoint(new Vector2(radius, 0), Vector2.Zero, i * rotationPer));
            }

            return new Polygon(vertices);
        }

        public static Polygon DeepCopy(Polygon otherPolygon)
        {
            if (otherPolygon is null)
                return null;

            Vector2[] vertices = new Vector2[otherPolygon.Vertices.Length];

            for (int i = 0; i < otherPolygon.Vertices.Length; i++)
            {
                vertices[i] = new Vector2(otherPolygon.Vertices[i].X, otherPolygon.Vertices[i].Y);
            }

            return new Polygon(vertices);
        }
    }

    public class ColoredPolygonJsonConverter : System.Text.Json.Serialization.JsonConverter<ColoredPolygon>
    {
        public override ColoredPolygon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of object");
            reader.Read();
            string text = reader.GetString() ?? throw new JsonException("String cannot be null");

            bool foundVertices = false;
            int lineThickness = 0;
            bool filled = false;
            bool foundColor = false;
            bool foundRadius = false;
            Polygon polygon = null;
            Color color = Color.White;
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
                    case "Filled":
                        filled = reader.GetBoolean();
                        break;
                    case "Color":
                        if (!foundColor)
                        {
                            var serializationOptionsWithConverter = new JsonSerializerOptions();
                            serializationOptionsWithConverter.Converters.Add(new ColorConverter());
                            color = JsonSerializer.Deserialize<Color>(ref reader, serializationOptionsWithConverter);
                            foundColor = true;
                        }
                        break;
                    case "LineThickness":
                        lineThickness = reader.GetInt32();
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
                    polygon = ColoredPolygon.RegularPolygon(radius, sides);
                }
                else
                {
                    throw new JsonException("Vertices not defined");
                }
            }

            ColoredPolygon cP = new ColoredPolygon(polygon, color, lineThickness, filled);
            return cP;
        }

        public override void Write(Utf8JsonWriter writer, ColoredPolygon value, JsonSerializerOptions options)
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

