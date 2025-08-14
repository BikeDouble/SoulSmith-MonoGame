using SoulSmith.Battle.Effects.Damage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers
{
    [JsonConverter(typeof(DurationStyleJsonConverter))]
    public enum DurationStyle
    {
        Null,
        Permanent,
        Rounds,
        MoveEnds,
        HostMoveEnds,
        HostGivesHits
    }

    public class DurationStyleJsonConverter : JsonConverter<DurationStyle>
    {
        public override DurationStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString()?.ToLower();
            return value switch
            {
                "permanent" => DurationStyle.Permanent,
                "rounds" => DurationStyle.Rounds,
                "moveends" => DurationStyle.MoveEnds,
                "hostmoveemds" => DurationStyle.HostMoveEnds,
                "hosthits" => DurationStyle.HostGivesHits,
                "hostgiveshits" => DurationStyle.HostGivesHits,
                _ => throw new JsonException($"Unknown DurationStyle value: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, DurationStyle value, JsonSerializerOptions options)
        {
            var strValue = value switch
            {
                DurationStyle.Permanent => "permanent",
                DurationStyle.Rounds => "rounds",
                DurationStyle.MoveEnds => "moveends",
                DurationStyle.HostMoveEnds => "hostmoveends",
                DurationStyle.HostGivesHits => "hostgiveshits",
                _ => throw new JsonException($"Unknown DurationStyle value: {value}")
            };
            writer.WriteStringValue(strValue);
        }
    }
}
