using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.UnitStats
{
    [JsonConverter(typeof(StatModStyleJsonConverter))]
    public enum StatModStyle
    {
        Null,
        Flat,
        AdditivePercent,
        MultiplicativePercent
    }

    public class StatModStyleJsonConverter : JsonConverter<StatModStyle>
    {
        public override StatModStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString()?.ToLower();
            return value switch
            {
                "flat" => StatModStyle.Flat,
                "additive" => StatModStyle.AdditivePercent,
                "additivepercent" => StatModStyle.AdditivePercent,
                "multiplicative" => StatModStyle.MultiplicativePercent,
                "multiplicativepercent" => StatModStyle.MultiplicativePercent,
                _ => StatModStyle.Null
            };
        }

        public override void Write(Utf8JsonWriter writer, StatModStyle value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
