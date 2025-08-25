using DynamicExpresso;
using SoulSmith.Asset;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Modifiers.Payload;
using SoulSmith.Battle.Modifiers.Stat;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects.Healing
{
    [JsonConverter(typeof(FormulaHealingEffectJsonConverter))]
    public class FormulaHealingEffect : VisualizedEffectBase, IEffect 
    {
        private Func<IReadOnlyUnit, IReadOnlyUnit, IReadOnlyCombat, IEffectOriginator, ResultBase, double> _parsedFormula;
        private AOETargetStyle? _aOEStyle; 
        private float _fractionOfHealingToSecondaryTargets;

        public FormulaHealingEffect(string formula, AOETargetStyle? aOEStyle, float fractionOfHealingToSecondaryTargets, TargetingStyle targetingStyle, EffectVisualizationFactory visualizationFactory, float additionalDelay, IEffect[] immediateAfterEffects) : base(targetingStyle, visualizationFactory, additionalDelay, immediateAfterEffects)
        {
            Interpreter interpreter = new Interpreter().Reference(typeof(IReadOnlyStatModifier)).Reference(typeof(ModifierResultBase)).Reference(typeof(IReadOnlyPayloadModifier)).Reference(typeof(IReadOnlyModAmountModifier));
            _parsedFormula = interpreter.ParseAsDelegate<Func<IReadOnlyUnit, IReadOnlyUnit, IReadOnlyCombat, IEffectOriginator, ResultBase, double>>(formula, "sender", "target", "combat", "originator", "parentResult");

            if (_parsedFormula == null)
            {
                throw new ArgumentException("The formula provided is not valid or could not be parsed.");
            }

            _aOEStyle = aOEStyle;
            _fractionOfHealingToSecondaryTargets = fractionOfHealingToSecondaryTargets;
        }

        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null)
        { 
            double healing = _parsedFormula(sender, target, combat, originator, parentEffectResult);

            if (!_aOEStyle.HasValue)
            {
                return new HealingPayload(sender, GetTrueTarget(sender, target, combat), (int)healing, parentEffectResult, this, originator, ImmediateAfterEffects);
            }
            else
            {
                IReadOnlyUnit trueTarget = GetTrueTarget(sender, target, combat);
                ICollection<IReadOnlyUnit> secondaryTargets = AOEPayloadBase.GetSecondaryTargets(trueTarget, combat, _aOEStyle.Value);
                return new AOEHealingPayload(sender, trueTarget, secondaryTargets, (int)healing, _fractionOfHealingToSecondaryTargets, parentEffectResult, this, originator, ImmediateAfterEffects);
            }
        }
    }

    public class FormulaHealingEffectJsonConverter : JsonConverter<FormulaHealingEffect>
    {
        public override FormulaHealingEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualizationFactory visualizationFactory = null;
            string formula = string.Empty;
            float additionalDelay = 0f;
            IEffect[] immediateAfterEffects = null;
            TargetingStyle targetingStyle = TargetingStyle.Target;
            AOETargetStyle? aOEStyle = null;
            float fractionOfHealingToSecondaryTargets = 1f;

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
                    case "FractionOfHealingToSecondaryTargets":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        fractionOfHealingToSecondaryTargets = reader.GetSingle();
                        reader.Read();
                        break;
                    default:
                        reader.Skip(); 
                        break;
                }
            }

            if (string.IsNullOrEmpty(formula)) throw new JsonException("Formula cannot be null or empty");

            return new FormulaHealingEffect(formula, aOEStyle, fractionOfHealingToSecondaryTargets, targetingStyle, visualizationFactory, additionalDelay, immediateAfterEffects);
        }

        public override void Write(Utf8JsonWriter writer, FormulaHealingEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
