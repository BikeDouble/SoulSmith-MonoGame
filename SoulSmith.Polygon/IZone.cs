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
    }

    public class IZoneJsonConverter : JsonConverter<IZone>
    {
        public override IZone? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            PolygonJsonConverter polygonConverter = new PolygonJsonConverter();
            return polygonConverter.Read(ref reader, typeToConvert, options);

            //TODO implement circles
        }

        public override void Write(Utf8JsonWriter writer, IZone value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
