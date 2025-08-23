using DynamicExpresso;
using SoulSmith.Asset;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers.Payload;
using SoulSmith.Battle.Modifiers.Stat;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects.Damage
{
    [JsonConverter(typeof(FormulaHitDamageEffectJsonConverter))]
    public class FormulaDamageEffect : VisualizedEffectBase, IEffect 
    {
        private Func<IReadOnlyUnit, IReadOnlyUnit, IReadOnlyCombat, IEffectOriginator, ResultBase, double> _parsedFormula;
        private DamageType _damageType;
        private AOETargetStyle? _aOEStyle; 
        private float _fractionOfDamageToSecondaryTargets;

        public FormulaDamageEffect(string formula, DamageType damageType, AOETargetStyle? aOEStyle, float fractionOfDamageToSecondaryTargets, TargetingStyle targetingStyle, EffectVisualizationFactory visualizationFactory, float additionalDelay, IEffect[] immediateAfterEffects) : base(targetingStyle, visualizationFactory, additionalDelay, immediateAfterEffects)
        {
            Interpreter interpreter = new Interpreter().Reference(typeof(IReadOnlyStatModifier)).Reference(typeof(ModifierResultBase)).Reference(typeof(IReadOnlyPayloadModifier));
            _parsedFormula = interpreter.ParseAsDelegate<Func<IReadOnlyUnit, IReadOnlyUnit, IReadOnlyCombat, IEffectOriginator, ResultBase, double>>(formula, "sender", "target", "combat", "originator", "parentResult");

            if (_parsedFormula == null)
            {
                throw new ArgumentException("The formula provided is not valid or could not be parsed.");
            }

            _damageType = damageType;
            _aOEStyle = aOEStyle;
            _fractionOfDamageToSecondaryTargets = fractionOfDamageToSecondaryTargets;
        }

        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null)
        { 
            double damage = _parsedFormula(sender, target, combat, originator, parentEffectResult);

            if (!_aOEStyle.HasValue)
            {
                return new DamagePayload(sender, GetTrueTarget(sender, target, combat), (int)damage, _damageType, parentEffectResult, this, originator, ImmediateAfterEffects);
            }
            else
            {
                IReadOnlyUnit trueTarget = GetTrueTarget(sender, target, combat);
                ICollection<IReadOnlyUnit> secondaryTargets = AOEPayloadBase.GetSecondaryTargets(trueTarget, combat, _aOEStyle.Value);
                return new AOEDamagePayload(sender, trueTarget, secondaryTargets, (int)damage, _damageType, _fractionOfDamageToSecondaryTargets, parentEffectResult, this, originator, ImmediateAfterEffects);
            }
        }
    }

    public class FormulaHitDamageEffectJsonConverter : JsonConverter<FormulaDamageEffect>
    {
        public override FormulaDamageEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualizationFactory visualizationFactory = null;
            string formula = string.Empty;
            float additionalDelay = 0f;
            DamageType damageType = DamageType.Null;
            IEffect[] immediateAfterEffects = null;
            TargetingStyle targetingStyle = TargetingStyle.Target;
            AOETargetStyle? aOEStyle = null;
            float fractionOfDamageToSecondaryTargets = 1f;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Delay":
                    case "AdditionalDelay":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        additionalDelay = reader.GetSingle();
                        reader.Read();
                        break;
                    case "Formula":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        formula = reader.GetString();
                        reader.Read();
                        break;
                    case "Visualization":
                    case "VisualizationFactory":
                    case "VisualizationKey":
                    case "VisualizationFactoryKey":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        string visKey = reader.GetString();
                        visualizationFactory = AssetManager.Instance.GetEffectVisualizationFactory<EffectVisualizationFactory>(visKey);
                        reader.Read();
                        break;
                    case "DamageType":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        damageType = JsonSerializer.Deserialize<DamageType>(ref reader, options);
                        reader.Read();
                        break;
                    case "ImmediateAfterEffects":
                        if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException("Expected start of an array");
                        immediateAfterEffects = JsonSerializer.Deserialize<IEffect[]>(ref reader, options);
                        reader.Read();
                        break;
                    case "TargetingStyle":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        targetingStyle = JsonSerializer.Deserialize<TargetingStyle>(ref reader, options);
                        reader.Read();
                        break;
                    case "AOEStyle":
                    case "AOETargetStyle":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        aOEStyle = JsonSerializer.Deserialize<AOETargetStyle>(ref reader, options);
                        reader.Read();
                        break;
                    case "FractionOfDamageToSecondaryTargets":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        fractionOfDamageToSecondaryTargets = reader.GetSingle();
                        reader.Read();
                        break;
                    default:
                        reader.Skip(); 
                        break;
                }
            }

            if (string.IsNullOrEmpty(formula)) throw new JsonException("Formula cannot be null or empty");
            if (damageType == DamageType.Null) throw new JsonException("DamageType cannot be Null");

            return new FormulaDamageEffect(formula, damageType, aOEStyle, fractionOfDamageToSecondaryTargets, targetingStyle, visualizationFactory, additionalDelay, immediateAfterEffects);
        }

        public override void Write(Utf8JsonWriter writer, FormulaDamageEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
