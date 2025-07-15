using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Drawing
{
    public class DrawableResourceKeyJsonConverter : JsonConverter<DrawableResourceKey>
    {
        public override DrawableResourceKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            string key = null;
            string type = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Key":
                            key = reader.GetString();
                            break;
                        case "Type":
                            type = reader.GetString();
                            break;
                    }
                }
            }

            if (key == null || type == null)
                throw new JsonException("Missing required properties.");

            return new DrawableResourceKey(key, type);
        }

        public override void Write(Utf8JsonWriter writer, DrawableResourceKey value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Key", value.Key);
            writer.WriteString("Type", value.Type);
            writer.WriteEndObject();
        }
    }
}
