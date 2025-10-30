using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Drawing.Animation;
using SoulSmith.Drawing.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Text
{
    [JsonConverter(typeof(TextBoxJsonConverter))]
    public class TextBox : IDisposable
    {
        public TextBoxFormat Format { get; private set; }
        private IAssetWrapper<IFontResource> _wrappedFont;
        public IFontResource Font { get { return _wrappedFont.Value; } }

        public TextBox(TextBoxFormat bounds, IAssetWrapper<IFontResource> fontResource)
        {
            Format = bounds ?? throw new ArgumentNullException(nameof(bounds));
            _wrappedFont = fontResource ?? throw new ArgumentNullException(nameof(fontResource));
        }

        public void Dispose()
        {
            _wrappedFont?.Dispose();
        }
    }

    public class TextBoxJsonConverter : JsonConverter<TextBox>
    {
        public override TextBox Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            TextBoxFormat format = null;
            IAssetWrapper<IFontResource> wrappedFont = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Format":
                    case "Bounds":
                        format = JsonSerializer.Deserialize<TextBoxFormat>(ref reader, options); 
                        reader.Read();
                        break;
                    case "Font":
                    case "FontKey":
                        string fontKey = reader.GetString();
                        wrappedFont = AssetManager.Instance.GetFontResource<IFontResource>(fontKey);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (wrappedFont == null || wrappedFont.Value == null) throw new ArgumentNullException(nameof(wrappedFont));
            if (format == null) throw new ArgumentNullException(nameof(format));

            return new TextBox(format, wrappedFont);
        }

        public override void Write(Utf8JsonWriter writer, TextBox value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
