using DynamicExpresso;
using System.Text.Json.Serialization;
using System.Text.Json;
using SoulSmith.Battle.Effect.Visualization;

namespace SoulSmith.Battle.Effect.Damage
{
    [JsonConverter(typeof(FormulaHitDamageEffectJsonConverter))]
    public class FormulaDamageEffect : VisualizedEffectBase, IEffect 
    {
        private Func<IReadOnlyUnit, IReadOnlyUnit, IReadOnlyCombat, double> _parsedFormula;
        private DamageType _damageType = DamageType.Hit;

        public FormulaDamageEffect(string formula, DamageType damageType, EffectVisualization visualization = null) : base(visualization)
        {
            Interpreter interpreter = new Interpreter();
            _parsedFormula = interpreter.ParseAsDelegate<Func<IReadOnlyUnit, IReadOnlyUnit, IReadOnlyCombat, double>>(formula, "sender", "target", "combat");
            _damageType = damageType;
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            double damage = _parsedFormula(sender, target, combat);

            return new EffectRequest(sender, target, _damageType, (int)damage);
        }
    }

    public class FormulaHitDamageEffectJsonConverter : JsonConverter<FormulaDamageEffect>
    {
        public override FormulaDamageEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualization visualization = null;
            string formula = string.Empty;
            DamageType damageType = DamageType.Null;

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
                    case "DamageType":
                        damageType = JsonSerializer.Deserialize<DamageType>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip(); 
                        break;
                }
            }

            return new FormulaDamageEffect(formula, damageType, visualization);
        }

        public override void Write(Utf8JsonWriter writer, FormulaDamageEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
