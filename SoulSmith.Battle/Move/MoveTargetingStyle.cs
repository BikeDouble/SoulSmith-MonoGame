using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Move
{
    [JsonConverter(typeof(MoveTargetingStyleJsonConverter))]
    public enum MoveTargetingStyle
    {
        Enemy,
        AllyOrSelf,
        Ally,
        None
    }

    public class MoveTargetingStyleJsonConverter : System.Text.Json.Serialization.JsonConverter<MoveTargetingStyle>
    {
        public override MoveTargetingStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected to decode a string");

            string value = reader.GetString();

            switch (value)
            {
                case "Enemy":
                    return MoveTargetingStyle.Enemy;
                case "AllyOrSelf":
                    return MoveTargetingStyle.AllyOrSelf;
                case "Ally":
                    return MoveTargetingStyle.Ally;
                default: 
                    return MoveTargetingStyle.None;
            }
        }

        public override void Write(Utf8JsonWriter writer, MoveTargetingStyle value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
