using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace SoulSmith.Drawing.Text
{
    [JsonConverter(typeof(LinePositioningJsonConverter))]
    public enum LinePositioning
    {
        Center,
        Top
    }

    internal class LinePositioningJsonConverter : JsonConverter<LinePositioning>
    {
        public override LinePositioning Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected to decode a string");
            string value = reader.GetString();
            switch (value.ToLower())
            {
                case "center":
                    return LinePositioning.Center;
                case "top":
                    return LinePositioning.Top;
                default:
                    throw new JsonException($"Could not convert {value} to LinePositioning");
            }
        }
        public override void Write(Utf8JsonWriter writer, LinePositioning value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
