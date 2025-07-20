using System.Text.Json.Serialization;
using System.Text.Json;
using SoulSmith.Battle.Effects.Visualization;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.UnitStats;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Drawing;
using SoulSmith.Battle.Modifiers.Stat;
using SoulSmith.Asset;
using System.Runtime;

namespace SoulSmith.Battle.Effects.Modifier
{
    [JsonConverter(typeof(ModifierEffectJsonConverter))]
    public class ModifierEffect : VisualizedEffectBase, IEffect
    {
        private ModifierFactory _modifierFactory;

        public ModifierEffect(
            ModifierFactory modifierFactory,
            EffectVisualizationFactory visualizationFactory = null
        ) : base(visualizationFactory)
        {
            _modifierFactory = modifierFactory;
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            IModifier modifier = _modifierFactory.CreateModifier();

            return new EffectRequest(sender, target, modifier);
        }
    }

    public class ModifierEffectJsonConverter : JsonConverter<ModifierEffect>
    {
        public override ModifierEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualizationFactory visualizationFactory = null;
            ModifierFactory modifierFactory = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
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

            return new ModifierEffect(
                modifierFactory,
                visualizationFactory
            );
        }

        public override void Write(Utf8JsonWriter writer, ModifierEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
