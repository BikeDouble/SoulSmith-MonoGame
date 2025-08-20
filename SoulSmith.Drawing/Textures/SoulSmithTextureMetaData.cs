using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Drawing.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Textures
{
    [JsonConverter(typeof(SoulSmithTextureMetaDataJsonConverter))]
    public class SoulSmithTextureMetaData
    {
        public SoulSmithTextureMetaData(SamplerState samplerState = null)
        {
            SamplerState = samplerState ?? SamplerState.PointClamp;
        }

        public SamplerState SamplerState { get; }
    }

    public class SoulSmithTextureMetaDataJsonConverter : System.Text.Json.Serialization.JsonConverter<SoulSmithTextureMetaData>
    {
        public override SoulSmithTextureMetaData Read(ref Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            SamplerState samplerState = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "SamplerState":
                        SamplerStateJsonConverter jsonConverter = new SamplerStateJsonConverter();
                        samplerState = jsonConverter.Read(ref reader, typeof(SamplerState), options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new SoulSmithTextureMetaData(samplerState);
        }
        public override void Write(Utf8JsonWriter writer, SoulSmithTextureMetaData value, System.Text.Json.JsonSerializerOptions options)
        {
            throw new NotImplementedException("Serialization is not implemented for SoulSmithTextureMetaData.");
        }
    }
}
