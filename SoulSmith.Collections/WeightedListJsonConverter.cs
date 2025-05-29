using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Collections
{
    public class WeightedListItemJsonConverter<T> : JsonConverter<WeightedListItem<T>>
    {
        public override WeightedListItem<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of an object");
            reader.Read();

            int weight = -1;
            bool itemFound = false;
            T item = default(T);
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
                reader.Read();
                switch (propertyName)
                {
                    case "Item":
                        item = JsonSerializer.Deserialize<T>(ref reader, options);
                        itemFound = true;
                        break;
                    case "Weight":
                        weight = reader.GetInt32();
                        break;
                }
                reader.Read();
            }
            if (!itemFound)
                throw new JsonException("No item specified");
            if (weight == -1)
                throw new JsonException("No weight specified");
            WeightedListItem<T> value = new WeightedListItem<T>(item, weight);
            return value;
        }

        public override void Write(Utf8JsonWriter writer, WeightedListItem<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Item");
            JsonSerializer.Serialize(writer, value._item);
            writer.WriteNumber("Weight", value._weight);
            writer.WriteEndObject();
        }
    }
}
