using SoulSmith.Battle.Effects.Damage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers.Effect
{
    [JsonConverter(typeof(EffectModifierTriggerStyleJsonConverter))]
    public enum EffectModifierTriggerStyle
    {
        OnTakingHitDamage,
        OnGivingHitDamage
    }

    public class EffectModifierTriggerStyleJsonConverter : JsonConverter<EffectModifierTriggerStyle>
    {
        public override EffectModifierTriggerStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString()?.ToLower();
            return value switch
            {
                "ontakinghitdamage" => EffectModifierTriggerStyle.OnTakingHitDamage,
                "ongivinghitdamage" => EffectModifierTriggerStyle.OnGivingHitDamage,
                _ => throw new JsonException($"Unknown DamageType value: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, EffectModifierTriggerStyle value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
