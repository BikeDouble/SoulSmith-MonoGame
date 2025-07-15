using System.Text.Json.Serialization;
using System.Text.Json;
using SoulSmith.Battle.Effect.Visualization;
using SoulSmith.UnitStats;
using SoulSmith.Battle.Modifier;
using SoulSmith.Drawing;

namespace SoulSmith.Battle.Effect.Modifier.Stat
{
    [JsonConverter(typeof(SimpleStaticRoundDurationStatModifierEffectJsonConverter))]
    public class SimpleStaticRoundDurationStatModifierEffect : VisualizedModifierEffectBase, IEffect 
    {
        private StatType _statType;
        private int _flatMod;
        private double _additiveMod;
        private double _multiplicativeMod;
        private int _duration;

        public SimpleStaticRoundDurationStatModifierEffect(
            StatType statType,
            int flatMod,
            double additiveMod,
            double multiplicativeMod,
            ModifierAlignment modifierAlignment = ModifierAlignment.Null,
            bool isModifierVisible = true,
            DrawableResourceKey modifierIconKey = null,
            EffectVisualization visualization = null
        ) : base(modifierAlignment, isModifierVisible, modifierIconKey, visualization)
        {
            _statType = statType;
            _flatMod = flatMod;
            _additiveMod = additiveMod;
            _multiplicativeMod = multiplicativeMod;
        }

        public EffectRequest GenerateEffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, EffectResult parentEffectResult = null)
        {
            StaticRoundDurationStatModifier modifier = new StaticRoundDurationStatModifier(
                _statType,
                _flatMod,
                _additiveMod,
                _multiplicativeMod,
                _duration,
                ModifierAlignment,
                IsModifierVisible,
                ModifierIconKey);

            return new EffectRequest(sender, target, modifier);
        }
    }

    public class SimpleStaticRoundDurationStatModifierEffectJsonConverter : JsonConverter<SimpleStaticRoundDurationStatModifierEffect>
    {
        public override SimpleStaticRoundDurationStatModifierEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectVisualization visualization = null;
            StatType statType = StatType.None;
            int flatMod = 0;
            double additiveMod = 0.0d;
            double multiplicativeMod = 1.0d;
            bool isModifierVisible = false;
            DrawableResourceKey modifierIconKey = null;
            int duration = 0;
            ModifierAlignment modifierAlignment = ModifierAlignment.Null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "IsModifierVisible":
                    case "ModifierVisible":
                        isModifierVisible = reader.GetBoolean();
                        reader.Read();
                        break;
                    case "StatType":
                        statType = JsonSerializer.Deserialize<StatType>(ref reader, options);
                        reader.Read();
                        break;
                    case "FlatMod":
                        flatMod = reader.GetInt32();
                        reader.Read();
                        break;
                    case "AdditiveMod":
                        additiveMod = reader.GetDouble();
                        reader.Read();
                        break;
                    case "MultiplicativeMod":
                        multiplicativeMod = reader.GetDouble();
                        reader.Read();
                        break;
                    case "Duration":
                        duration = reader.GetInt32();
                        reader.Read();
                        break;
                    case "ModifierAlignment":
                        modifierAlignment = JsonSerializer.Deserialize<ModifierAlignment>(ref reader, options);
                        reader.Read();
                        break;
                    case "ModifierIconKey":
                        modifierIconKey = JsonSerializer.Deserialize<DrawableResourceKey>(ref reader, options);
                        reader.Read();
                        break;
                    case "Visualization":
                        visualization = JsonSerializer.Deserialize<EffectVisualization>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip(); 
                        break;
                }
            }

            return new SimpleStaticRoundDurationStatModifierEffect(statType, flatMod, additiveMod, multiplicativeMod, modifierAlignment, isModifierVisible, modifierIconKey, visualization);
        }

        public override void Write(Utf8JsonWriter writer, SimpleStaticRoundDurationStatModifierEffect value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
