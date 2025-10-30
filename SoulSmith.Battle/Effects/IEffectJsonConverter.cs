using System.Text.Json.Serialization;
using System.Text.Json;
using SoulSmith.Battle.Effects.Modifier;
using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Healing;

namespace SoulSmith.Battle.Effects
{
    public class IEffectJsonConverter : JsonConverter<IEffect>
    {
        public override IEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            if (reader.GetString() != "Type") throw new JsonException("Expected type of effect");

            reader.Read();

            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected name of Effects type");

            string effectType = reader.GetString();

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            if (reader.GetString() != "Effect") throw new JsonException("Expected effect");

            reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");

            IEffect value = null;

            switch (effectType)
            {
                case "DamageFormula":
                case "FormulaDamage":
                    value = JsonSerializer.Deserialize<FormulaDamageEffect>(ref reader, options);
                    reader.Read();
                    break;
                case "HealingFormula":
                case "FormulaHealing":
                    value = JsonSerializer.Deserialize<FormulaHealingEffect>(ref reader, options);
                    reader.Read();
                    break;
                case "Modifier":
                    value = JsonSerializer.Deserialize<AddModifierEffect>(ref reader, options);
                    reader.Read();
                    break;
                case "ReapplyStatModifier":
                    value = JsonSerializer.Deserialize<ReapplyStaticStatModifierEffect>(ref reader, options);
                    reader.Read();
                    break;
                case "ReapplyPayloadModifier":
                    value = JsonSerializer.Deserialize<ReapplyPayloadModifierEffect>(ref reader, options);
                    reader.Read();
                    break;
                case "RemoveModifier": // The RemoveModifier effect will never be deserialized from json, only called in code, therefore reroute to RemoveModifierWithMergeKey
                case "RemoveModifierWithMergeKey":
                    value = JsonSerializer.Deserialize<RemoveModifierWithMergeKeyEffect>(ref reader, options);
                    reader.Read();
                    break;
                default:
                    throw new JsonException($"Unexpected type {effectType}. Type is either misspelled or does not exist.");
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, IEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
