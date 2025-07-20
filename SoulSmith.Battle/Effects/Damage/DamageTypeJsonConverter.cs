using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SoulSmith.Battle.Effects.Damage;

namespace SoulSmith.Battle.Effects.Damage
{
    public class DamageTypeJsonConverter : JsonConverter<DamageType>
    {
        public override DamageType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString()?.ToLower();
            return value switch
            {
                "null" => DamageType.Null,
                "hit" => DamageType.Hit,
                "essence" => DamageType.Essence,
                _ => throw new JsonException($"Unknown DamageType value: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, DamageType value, JsonSerializerOptions options)
        {
            var strValue = value switch
            {
                DamageType.Null => "null",
                DamageType.Hit => "hit",
                DamageType.Essence => "essence",
                _ => throw new JsonException($"Unknown DamageType value: {value}")
            };
            writer.WriteStringValue(strValue);
        }
    }
}
