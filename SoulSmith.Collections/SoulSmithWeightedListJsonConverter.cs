using SoulSmith.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Collections
{
    public class SoulSmithWeightedListJsonConverter<T> : JsonConverter<SoulSmithWeightedList<T>>
    {
        public override SoulSmithWeightedList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected start of an array");
            var serializationOptionsWithConverter = new JsonSerializerOptions();
            serializationOptionsWithConverter.Converters.Add(new WeightedListItemJsonConverter<T>());
            WeightedListItem<T>[] items = JsonSerializer.Deserialize<WeightedListItem<T>[]>(ref reader, serializationOptionsWithConverter);
            return new SoulSmithWeightedList<T>(items, Rand.Random);
        }

        public override void Write(Utf8JsonWriter writer, SoulSmithWeightedList<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (T item in value)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Item");
                JsonSerializer.Serialize<T>(writer, item);
                writer.WriteNumber("Weight", value.GetWeightOf(item));
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}
