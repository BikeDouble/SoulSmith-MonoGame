using System.Text.Json.Serialization;
using System.Text.Json;
using SoulSmith.Battle.Effect.Visualization;
using SoulSmith.UnitStats;
using SoulSmith.Battle.Modifier;

namespace SoulSmith.Battle.Effect
{
    [JsonConverter(typeof(SimpleStaticRoundDurationStatModifierEffectJsonConverter))]
    public class SimpleStaticRoundDurationStatModifierEffect : VisualizedEffect, IEffect 
    {
        private StatType _statType;
        private int _flatMod;
        private double _additiveMod;
        private double _multiplicativeMod;

        public SimpleStaticRoundDurationStatModifierEffect(StatType statType, int flatMod, double additiveMod, double multiplicativeMod, EffectVisualization visualization = null) : base(visualization) 
        {
            _statType = statType;
            _flatMod = flatMod;
            _additiveMod = additiveMod;
            _multiplicativeMod = multiplicativeMod;
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            StaticRoundDurationStatModifier modifier = new StaticRoundDurationStatModifier(_statType, _flatMod, _additiveMod, _multiplicativeMod);

            return new EffectRequest(sender, target, modifier);
        }
    }

    public class SimpleStaticRoundDurationStatModifierEffectJsonConverter : JsonConverter<SimpleStaticRoundDurationStatModifierEffect>
    {
        public override HitDamageFormulaEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualization visualization = null;
            string formula = string.Empty;
            bool gainDecay = true;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Formula":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        formula = reader.GetString();
                        reader.Read();
                        break;
                    case "Visualization":
                        visualization = JsonSerializer.Deserialize<EffectVisualization>(ref reader, options);
                        reader.Read();
                        break;
                    case "GainDecay":
                        if (!((reader.TokenType == JsonTokenType.True) || (reader.TokenType == JsonTokenType.False))) throw new JsonException("Expected boolean");
                        gainDecay = reader.GetBoolean();
                        reader.Read();
                        break;
                    default:
                        reader.Skip(); 
                        break;
                }
            }

            return new HitDamageFormulaEffect(formula, gainDecay, visualization);
        }

        public override void Write(Utf8JsonWriter writer, HitDamageFormulaEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
