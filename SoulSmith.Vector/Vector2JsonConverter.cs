using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Vector
{
    public class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of an object");
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name");

            float x = 0;
            float y = 0;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
                reader.Read();
                switch (propertyName)
                {
                    case "X":
                        x = reader.GetSingle();
                        break;
                    case "Y":
                        y = reader.GetSingle();
                        break;
                    default:
                        throw new JsonException($"Unknown property: {propertyName}");
                }
                reader.Read();
            }
            Vector2 vector2 = new Vector2(x, y);
            return vector2;
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteEndObject();
        }
    }

    public class Vector2ArrayConverter : JsonConverter<Vector2[]> 
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
