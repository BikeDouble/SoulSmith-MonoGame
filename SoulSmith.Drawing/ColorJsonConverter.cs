using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Drawing
{
    public class ColorJsonConverter : System.Text.Json.Serialization.JsonConverter<Microsoft.Xna.Framework.Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected start of an array");
            reader.Read();
            int[] color = { 255, 255, 255, 255 };
            int i = 0;

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                if (i >= 4)
                    throw new JsonException("Expected maximum of 4 values");
                color[i] = reader.GetInt32();
                i++;
                reader.Read();
            }

            return new Color(color[0], color[1], color[2], color[3]);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.R);
            writer.WriteNumberValue(value.G);
            writer.WriteNumberValue(value.B);
            writer.WriteNumberValue(value.A);
            writer.WriteEndArray();
        }
    }
}
