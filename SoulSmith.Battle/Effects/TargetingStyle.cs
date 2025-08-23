using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects
{
    [JsonConverter(typeof(TargetingStyleJsonConverter))]
    public enum TargetingStyle
    {
        Target,
        Sender,
        AcrossFromSender,
        AnyEnemy,
        Special
    }

    public class TargetingStyleJsonConverter : JsonConverter<TargetingStyle>
    {
        public override TargetingStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected string value for {nameof(TargetingStyle)}, but got {reader.TokenType}");
            }

            string value = reader.GetString().ToLowerInvariant();

            return value switch
            {
                "target" => TargetingStyle.Target,
                "sender" => TargetingStyle.Sender,
                "self" => TargetingStyle.Sender,
                "acrossfromsender" => TargetingStyle.AcrossFromSender,
                "across" => TargetingStyle.AcrossFromSender,
                "anyenemy" => TargetingStyle.AnyEnemy,
                _ => throw new JsonException($"Unknown {nameof(TargetingStyle)} value: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, TargetingStyle value, JsonSerializerOptions options)
        {
            string stringValue = value switch
            {
                TargetingStyle.Target => "target",
                TargetingStyle.Sender => "sender",
                TargetingStyle.AcrossFromSender => "acrossfromsender",
                TargetingStyle.AnyEnemy => "anyenemy",
                TargetingStyle.Special => "special",
                _ => throw new JsonException($"Unknown {nameof(TargetingStyle)} value: {value}")
            };

            writer.WriteStringValue(stringValue);
        }
    }
}
