using SoulSmith.Asset;
using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers;
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
    [JsonConverter(typeof(RampageRefundEffectJsonConverter))]
    public class RampageRefundEffect : VisualizedEffectBase, IEffect
    {
        public const string ANGERINNATESTACKSMERGEKEY = "AngerInnateStack";

        private float _portionOfModToRefund;

        public RampageRefundEffect(
            float portionOfModToRefund,
            EffectVisualizationFactory visualizationFactory,
            float additionalDelay,
            IEffect[] immediateAfterEffects
        ) : base(visualizationFactory, additionalDelay, immediateAfterEffects)
        {
            _portionOfModToRefund = portionOfModToRefund;
        }

        public Payload GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, Result parentEffectResult = null)
        {
            if (!(parentEffectResult is DamageResult damageResult)) return null;

            if (!damageResult.KilledTarget) return null;

            IReadOnlyModifier currentModifier = sender.ReadOnlyStats.GetReadOnlyModifier(ANGERINNATESTACKSMERGEKEY);

            if (!(currentModifier is IReadOnlyStatModifier currentStatMod)) return null;

            double refundedAmount = currentStatMod.ModAmount * _portionOfModToRefund;

            IModifier modifier = new StaticStatModifier(
                currentStatMod.StatType,
                currentStatMod.ModStyle,
                refundedAmount,
                1,
                currentStatMod.DurationStyle,
                currentStatMod.Alignment,
                currentStatMod.IsVisible,
                currentStatMod.IconKey,
                currentStatMod.FriendlyName,
                currentStatMod.Description,
                currentStatMod.MergeKey);

            AddModifierPayload payload = new AddModifierPayload(sender, sender, modifier, parentEffectResult, this, ImmediateAfterEffects);

            return payload;
        }
    }

    public class RampageRefundEffectJsonConverter : JsonConverter<RampageRefundEffect>
    {
        public override RampageRefundEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualizationFactory visualizationFactory = null;
            float? portionOfModToRefund = null;
            float additionalDelay = 0f;
            IEffect[] immediateAfterEffects = null;

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
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (!portionOfModToRefund.HasValue) throw new JsonException("Portion of mod to refund is required");

            return new RampageRefundEffect(portionOfModToRefund.Value, visualizationFactory, additionalDelay, immediateAfterEffects);
        }

        public override void Write(Utf8JsonWriter writer, RampageRefundEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}