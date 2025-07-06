using Microsoft.Xna.Framework;
using SoulSmith.Asset;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SoulSmith.Drawing;
public class TrackedIDrawableResourceJsonConverter : JsonConverter<IAssetWrapper<IDrawableResource>>
{
    public override IAssetWrapper<IDrawableResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

        reader.Read();

        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

        if (reader.GetString() != "Type") throw new JsonException("Expected type of resource");

        reader.Read();

        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected name of resource type");

        string type = reader.GetString();

        reader.Read();

        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

        if (reader.GetString() != "Key") throw new JsonException("Expected resource asset key");

        reader.Read();

        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected start of object");

        string key = reader.GetString();

        reader.Read();

        IAssetWrapper<IDrawableResource> value = DrawHelpers.GetDrawableResource(key, type);

        return value;
    }

    public override void Write(Utf8JsonWriter writer, IAssetWrapper<IDrawableResource> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

