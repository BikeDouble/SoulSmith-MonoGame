using Microsoft.Xna.Framework;
using SoulSmith.Core;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace SoulSmith.Shapes
{
    [JsonConverter(typeof(IZoneJsonConverter))]
    public interface IZone
    {
        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation);
        public bool ContainsLocal(Vector2 point);
        public Vector2 GetRandomLocalPoint();
        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition transformation);
        public float GetAreaLocal();
        public float GetHeightLocal();
        public float GetWidthLocal();
    }

    public class IZoneJsonConverter : JsonConverter<IZone>
    {
        public override IZone? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of an object");

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name"); 

            if (reader.GetString() != "Type")
                throw new JsonException("Expected property name 'Type'");

            reader.Read();

            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected string for type name");

            string typeName = reader.GetString() ?? throw new JsonException("Type name cannot be null");

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected start of an object for zone data");

            string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");

            if (propertyName != "Zone" && propertyName != "Data" && propertyName != "ZoneData")
                throw new JsonException("Expected property name 'Zone'");

            reader.Read();

            IZone zone;

            switch (typeName)
            {
                case "Polygon":
                case "polygon":
                    zone = JsonSerializer.Deserialize<Polygon>(ref reader, options);
                    reader.Read();
                    break;
                default:
                    throw new JsonException($"Unknown or not implemented type: {typeName}");
            }

            return zone;
        }

        public override void Write(Utf8JsonWriter writer, IZone value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
