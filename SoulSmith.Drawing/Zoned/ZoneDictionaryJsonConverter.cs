using SoulSmith.Asset;
using SoulSmith.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Drawing.Zoned
{
    public class ZoneDictionaryJsonConverter : JsonConverter<Dictionary<string, IZone>>
    {
        public override Dictionary<string, IZone> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected StartArray token");

            Dictionary<string, IZone> zones = new Dictionary<string, IZone>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException("Expected StartObject for Vector2");

                string key = null;
                IZone zone = null;

                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType != JsonTokenType.PropertyName)
                        continue;

                    string propName = reader.GetString();
                    reader.Read();

                    switch (propName)
                    {
                        case "key":
                        case "Key":
                            key = reader.GetString();
                            break;
                        case "zone":
                        case "Zone":
                            zone = JsonSerializer.Deserialize<IZone>(ref reader, options);
                            break;
                    }
                }

                if (key != null && zone != null)
                {
                    zones[key] = zone;
                }
            }

            return zones;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, IZone> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Writing ZoneDictionary is not implemented yet.");
        }
    }
}
