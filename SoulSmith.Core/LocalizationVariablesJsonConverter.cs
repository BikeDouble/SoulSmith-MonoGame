using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SoulSmith.Core
{
    public class LocalizationVariablesJsonConverter : JsonConverter<Dictionary<string, string>>
    {
        public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            Dictionary<string, string> variables = new Dictionary<string, string>();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                string value = string.Empty;

                switch(reader.TokenType)
                {
                    case JsonTokenType.String:
                        value = reader.GetString();
                        break;
                    case JsonTokenType.Number:
                        value = reader.GetUInt64().ToString();
                        break;
                }

                variables.Add(propertyName, value);

                reader.Read();
            }

            return variables;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
