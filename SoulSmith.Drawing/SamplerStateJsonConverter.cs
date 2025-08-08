using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Drawing
{
    public class SamplerStateJsonConverter : JsonConverter<SamplerState>
    {
        public override SamplerState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected to decode a string");

            string value = reader.GetString();

            switch (value.ToLower())
            {
                case "pointclamp":
                    return SamplerState.PointClamp;
                case "pointwrap":
                    return SamplerState.PointWrap;
                case "linearclamp":
                    return SamplerState.LinearClamp;
                case "linearwrap":
                    return SamplerState.LinearWrap;
                case "anisotropicclamp":
                    return SamplerState.AnisotropicClamp;
                case "anisotropicwrap":
                    return SamplerState.AnisotropicWrap;
                default:
                    return null;
            }
        }

        public override void Write(Utf8JsonWriter writer, SamplerState value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
