using SoulSmith.Asset;
using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Modifiers.Payload;
using SoulSmith.Battle.Modifiers.Stat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Modifier
{
    [JsonConverter(typeof(ReapplyPayloadModifierEffectJsonConverter))]
    public class ReapplyPayloadModifierEffect : VisualizedEffectBase, IEffect
    {
        private float _portionOfModToRefund;

        public ReapplyPayloadModifierEffect(
            float portionOfModToRefund,
            TargetingStyle targetingStyle,
            EffectVisualizationFactory visualizationFactory,
            float additionalDelay,
            IEffect[] immediateAfterEffects
        ) : base(targetingStyle, visualizationFactory, additionalDelay, immediateAfterEffects)
        {
            _portionOfModToRefund = portionOfModToRefund;
        }

        public PayloadBase GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentEffectResult = null)
        {
            if (!(parentEffectResult is RemoveModifierResult removeResult)) return null;

            IReadOnlyModifier currentModifier = removeResult.Modifier;

            if (!(currentModifier is IReadOnlyPayloadModifier currentPayloadMod)) return null;

            float refundedAmount = currentPayloadMod.ModAmount * _portionOfModToRefund;

            IModifier modifier = new StaticPayloadModifier(
                currentPayloadMod.TriggerStyle,
                currentPayloadMod.ModStyle,
                refundedAmount,
                1,
                currentPayloadMod.DurationStyle,
                currentPayloadMod.Alignment,
                originator,
                currentPayloadMod.IsVisible,
                currentPayloadMod.IconKey,
                currentPayloadMod.FriendlyName,
                currentPayloadMod.Description,
                currentPayloadMod.MergeKey);

            AddModifierPayload payload = new AddModifierPayload(sender, GetTrueTarget(sender, target, combat), modifier, parentEffectResult, this, originator, ImmediateAfterEffects);

            return payload;
        }
    }

    public class ReapplyPayloadModifierEffectJsonConverter : JsonConverter<ReapplyPayloadModifierEffect>
    {
        public override ReapplyPayloadModifierEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualizationFactory visualizationFactory = null;
            float? portionOfModToRefund = null;
            float additionalDelay = 0f;
            IEffect[] immediateAfterEffects = null;
            TargetingStyle targetingStyle = TargetingStyle.Target;

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
                    case "Visualization":
                    case "VisualizationFactory":
                    case "VisualizationKey":
                    case "VisualizationFactoryKey":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        string visKey = reader.GetString();
                        visualizationFactory = AssetManager.Instance.GetEffectVisualizationFactory<EffectVisualizationFactory>(visKey);
                        reader.Read();
                        break;
                    case "PortionOfModToRefund":
                        portionOfModToRefund = reader.GetSingle();
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

            if (!portionOfModToRefund.HasValue) throw new JsonException("Portion of mod to refund is required");

            return new ReapplyPayloadModifierEffect(portionOfModToRefund.Value, targetingStyle, visualizationFactory, additionalDelay, immediateAfterEffects);
        }

        public override void Write(Utf8JsonWriter writer, ReapplyPayloadModifierEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}