using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Damage
{
    [JsonConverter(typeof(DamageTypeJsonConverter))]
    public enum DamageType
    {
        Null,
        Hit,
        Essence
    }
}
