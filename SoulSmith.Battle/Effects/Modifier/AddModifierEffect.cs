using SoulSmith.Asset;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects.Modifier
{
    [JsonConverter(typeof(AddModifierEffectJsonConverter))]
    public class AddModifierEffect : VisualizedEffectBase, IEffect
    {
        private ModifierFactory _modifierFactory;

        public AddModifierEffect(
            ModifierFactory modifierFactory,
            EffectVisualizationFactory visualizationFactory,
            float additionalDelay
        ) : base(visualizationFactory, additionalDelay)
        {
            _modifierFactory = modifierFactory;
        }

        public Payload GeneratePayload(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, Result parentEffectResult = null)
        {
            IModifier modifier = _modifierFactory.CreateModifier();

            return new AddModifierPayload(sender, target, modifier, parentEffectResult);
        }
    }

    public class AddModifierEffectJsonConverter : JsonConverter<AddModifierEffect>
    {
        public override AddModifierEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualizationFactory visualizationFactory = null;
            ModifierFactory modifierFactory = null;
            float additionalDelay = 0f;

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
                    case "ModifierFactoryKey":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        string modifierKey = reader.GetString();
                        modifierFactory = AssetManager.Instance.GetModifierFactory<ModifierFactory>(modifierKey);
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
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (modifierFactory == null)
            {
                throw new JsonException("ModifierFactory is required but was not provided.");
            }

            return new AddModifierEffect(
                modifierFactory,
                visualizationFactory,
                additionalDelay
            );
        }

        public override void Write(Utf8JsonWriter writer, AddModifierEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
