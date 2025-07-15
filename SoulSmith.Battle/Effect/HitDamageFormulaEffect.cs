using DynamicExpresso;
using SoulSmith.Battle;
using System.Text.Json.Serialization;
using System.Text.Json;
using SoulSmith.Battle.Effect.Visualization;

namespace SoulSmith.Battle.Effect
{
    [JsonConverter(typeof(HitDamageFormulaEffectJsonConverter))]
    public class HitDamageFormulaEffect : VisualizedEffectBase, IEffect 
    {
        private Func<IReadOnlyUnit, IReadOnlyUnit, IReadOnlyCombat, double> _parsedFormula;
        private EffectVisualization _visualiztion = null;

        public HitDamageFormulaEffect(string formula, bool gainDecay = true, EffectVisualization visualization = null) : base(visualization)
        {
            Interpreter interpreter = new Interpreter();
            _parsedFormula = interpreter.ParseAsDelegate<Func<IReadOnlyUnit, IReadOnlyUnit, IReadOnlyCombat, double>>(formula, "sender", "target", "combat");
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            double damage = _parsedFormula(sender, target, combat);

            return new EffectRequest(sender, target, DamageType.Hit, (int)damage);
        }
    }

    public class HitDamageFormulaEffectJsonConverter : JsonConverter<HitDamageFormulaEffect>
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
