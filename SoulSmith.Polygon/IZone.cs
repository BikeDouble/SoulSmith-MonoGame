using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Core;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace SoulSmith.Shapes
{
    [JsonConverter(typeof(IZoneJsonConverter))]
    public interface IZone
    {
        public bool Contains(Vector2 point, IReadOnlyPosition transformation);
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
