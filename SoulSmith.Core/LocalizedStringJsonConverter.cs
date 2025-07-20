using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Core
{
    public class LocalizedStringJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            Dictionary<string, string> variables = new Dictionary<string, string>();
            string localizationKey = string.Empty;

            string propertyName = string.Empty;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                propertyName = reader.GetString();

                reader.Read();

                string value = string.Empty;

                switch (propertyName)
                {
                    case "Variables":
                    case "LocalizationVariables":
                        LocalizationVariablesJsonConverter converter = new LocalizationVariablesJsonConverter();
                        variables = converter.Read(ref reader, typeof(Dictionary<string, string>), options);
                        reader.Read();
                        break;
                    case "LocalizationKey":
                        localizationKey = reader.GetString();
                        reader.Read();
                        break;
                }
            }

            return "Localization not yet implemented.";
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
