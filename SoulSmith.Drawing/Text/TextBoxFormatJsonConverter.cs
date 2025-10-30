using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Drawing.Text
{
    internal class TextBoxFormatJsonConverter : JsonConverter<TextBoxFormat>
    {
        public override TextBoxFormat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int x = 0, y = 0, width = 0, height = 0, bottomXOffset = 0, spaceBetweenLines = 0, borderPadding = 0, fontSize = 0;
            LinePositioning linePositioning = LinePositioning.Center;

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "X": x = reader.GetInt32(); break;
                        case "Y": y = reader.GetInt32(); break;
                        case "Width": width = reader.GetInt32(); break;
                        case "Height": height = reader.GetInt32(); break;
                        case "BottomXOffset": bottomXOffset = reader.GetInt32(); break;
                        case "SpaceBetweenLines": spaceBetweenLines = reader.GetInt32(); break;
                        case "BorderPadding": borderPadding = reader.GetInt32(); break;
                        case "FontSize": fontSize = reader.GetInt32(); break;
                        case "LinePositioning": linePositioning = JsonSerializer.Deserialize<LinePositioning>(ref reader, options); break;
                    }
                }
            }

            TextBoxFormat bounds = new TextBoxFormat(x, y, width, height, bottomXOffset, spaceBetweenLines, borderPadding, fontSize, linePositioning);
            return bounds;
        }

        public override void Write(Utf8JsonWriter writer, TextBoxFormat value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
