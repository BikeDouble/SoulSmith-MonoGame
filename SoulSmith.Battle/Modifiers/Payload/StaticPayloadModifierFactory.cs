using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Modifiers.Effect;
using SoulSmith.UnitStats;
using System.Text.Json;
using System.Text.Json.Serialization;
using SoulSmith.Localization;

namespace SoulSmith.Battle.Modifiers.Payload
{
    [JsonConverter(typeof(StaticPayloadModifierFactoryJsonConverter))]
    public class StaticPayloadModifierFactory : ModifierFactory
    {
        public StaticPayloadModifierFactory(
            EffectModifierTriggerStyle triggerStyle,
            StatModStyle statModStyle,
            float modAmount,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            bool isVisible,
            string iconKey,
            string friendlyName,
            string description,
            string mergeKey)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description, mergeKey)
        {
            ModStyle = statModStyle;
            ModAmount = modAmount;
            TriggerStyle = triggerStyle;
        }

        public EffectModifierTriggerStyle TriggerStyle { get; private set; }
        public StatType StatType { get; private set; }
        public StatModStyle ModStyle { get; private set; }
        public float ModAmount { get; private set; }

        public override IModifier CreateModifier(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentResult)
        {
            return new StaticPayloadModifier(TriggerStyle, ModStyle, ModAmount, Duration, DurationStyle, ModifierAlignment, originator, IsModifierVisible, ModifierIconKey, FriendlyName, Description, MergeKey);
        }
    }

    public class StaticPayloadModifierFactoryJsonConverter : JsonConverter<StaticPayloadModifierFactory>
    {
        public override StaticPayloadModifierFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            EffectModifierTriggerStyle? triggerStyle = null;
            StatModStyle modStyle = StatModStyle.Null;
            float? modAmount = null;
            int duration = 0;
            DurationStyle? durationStyle = null;
            ModifierAlignment? modifierAlignment = null;
            bool? isModifierVisible = null;
            string modifierIconKey = null;
            string friendlyName = "Unnamed";
            string description = string.Empty;
            string mergeKey = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "TriggerStyle":
                    case "EffectModifierTriggerStyle":
                        triggerStyle = JsonSerializer.Deserialize<EffectModifierTriggerStyle>(ref reader, options);
                        reader.Read();
                        break;
                    case "StatModStyle":
                    case "ModStyle":
                        modStyle = JsonSerializer.Deserialize<StatModStyle>(ref reader, options);
                        reader.Read();
                        break;
                    case "Mod":
                    case "ModAmount":
                        modAmount = reader.GetSingle();
                        reader.Read();
                        break;
                    case "Duration":
                        duration = reader.GetInt32();
                        reader.Read();
                        break;
                    case "DurationStyle":
                        durationStyle = JsonSerializer.Deserialize<DurationStyle>(ref reader, options);
                        reader.Read();
                        break;
                    case "ModifierAlignment":
                    case "Alignment":
                        modifierAlignment = JsonSerializer.Deserialize<ModifierAlignment>(ref reader, options);
                        reader.Read();
                        break;
                    case "IsModifierVisible":
                    case "ModifierVisible":
                    case "Visible":
                        isModifierVisible = reader.GetBoolean();
                        reader.Read();
                        break;
                    case "ModifierIconKey":
                    case "IconKey":
                        modifierIconKey = reader.GetString();
                        reader.Read();
                        break;
                    case "FriendlyName":
                        friendlyName = reader.GetString() ?? "Unnamed";
                        reader.Read();
                        break;
                    case "Description":
                        LocalizedStringJsonConverter localizedStringConverter = new LocalizedStringJsonConverter();
                        description = localizedStringConverter.Read(ref reader, typeof(string), options);
                        reader.Read();
                        break;
                    case "MergeKey":
                        mergeKey = reader.GetString();
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (modStyle == StatModStyle.Null) throw new JsonException("Expected 'StatModStyle' property to be present.");
            if (!modAmount.HasValue) throw new JsonException("Expected 'ModAmount' property to be present.");
            if (isModifierVisible == null) throw new JsonException("Expected 'IsModifierVisible' property to be present.");
            if (durationStyle == null) throw new JsonException("Expected 'DurationStyle' property to be present.");
            if (modifierAlignment == null) throw new JsonException("Expected 'ModifierAlignment' property to be present.");
            if ((modifierIconKey == null) && (isModifierVisible.Value)) throw new JsonException("Expected 'ModifierIconKey' property to be present.");
            if (triggerStyle == null) throw new JsonException("Expected 'TriggerStyle' property to be present.");

            return new StaticPayloadModifierFactory(triggerStyle.Value, modStyle, modAmount.Value, duration, durationStyle.Value, modifierAlignment.Value, isModifierVisible.Value, modifierIconKey, friendlyName, description, mergeKey);
        }

        public override void Write(Utf8JsonWriter writer, StaticPayloadModifierFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

