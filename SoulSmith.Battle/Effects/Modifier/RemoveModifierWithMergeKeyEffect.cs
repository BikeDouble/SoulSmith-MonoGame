using SoulSmith.Asset;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Modifier
{
    [JsonConverter(typeof(RemoveModifierWithMergeKeyEffectJsonConverter))]
    public class RemoveModifierWithMergeKeyEffect : VisualizedEffectBase, IEffect
    {
        private string _mergeKey;

        public RemoveModifierWithMergeKeyEffect(
            string mergeKey,
            TargetingStyle targetingStyle,
            EffectVisualizationFactory visualizationFactory,
            float additionalDelay,
            IEnumerable<IEffect> immediateAfterEffects = null
        ) : base(targetingStyle, visualizationFactory, additionalDelay, immediateAfterEffects)
        {
            _mergeKey = mergeKey;
        }

        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null)
        {
            IReadOnlyUnit trueTarget = GetTrueTarget(sender, target, combat);
            if (trueTarget == null) return null;

            IReadOnlyModifier modifier = trueTarget.ReadOnlyStats.GetReadOnlyModifier(_mergeKey);
            if (modifier == null) return null;

            return new RemoveModifierPayload(sender, modifier.Host, modifier, parentEffectResult, this, originator, ImmediateAfterEffects);
        }
    }

    public class RemoveModifierWithMergeKeyEffectJsonConverter : JsonConverter<RemoveModifierWithMergeKeyEffect>
    {
        public override RemoveModifierWithMergeKeyEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualizationFactory visualizationFactory = null;
            float additionalDelay = 0f;
            IEffect[] immediateAfterEffects = null;
            TargetingStyle targetingStyle = TargetingStyle.Target;
            string mergeKey = null;

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
                    case "ModifierKey":
                    case "MergeKey":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        mergeKey = reader.GetString();
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
                        targetingStyle = JsonSerializer.Deserialize<TargetingStyle>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new RemoveModifierWithMergeKeyEffect(
                mergeKey,
                targetingStyle,
                visualizationFactory,
                additionalDelay,
                immediateAfterEffects
            );
        }

        public override void Write(Utf8JsonWriter writer, RemoveModifierWithMergeKeyEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
