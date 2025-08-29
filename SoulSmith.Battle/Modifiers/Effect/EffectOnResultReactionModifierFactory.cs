using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Results;
using System.Text.Json;
using System.Text.Json.Serialization;
using SoulSmith.Localization;

namespace SoulSmith.Battle.Modifiers.Effect
{
    [JsonConverter(typeof(EffectOnGivingHitModifierFactoryJsonConverter))]
    public class EffectOnResultReactionModifierFactory : ModifierFactory
    {
        public EffectOnResultReactionModifierFactory(
            IEffect effect,
            Priority effectPriority,
            EffectModifierTriggerStyle triggerStyle,
            string reactionTargetModifierMergeKey,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            bool isVisible,
            string iconKey,
            string friendlyName,
            string description,
            string statusText)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description)
        {
            Effect = effect;
            StatusText = statusText;
            EffectPriority = effectPriority;
            TriggerStyle = triggerStyle;
            ReactionTargetModifierMergeKey = reactionTargetModifierMergeKey;
        }

        public IEffect Effect { get; private set; }
        public string StatusText { get; private set; }
        public Priority EffectPriority { get; private set; }
        public EffectModifierTriggerStyle TriggerStyle { get; private set; }
        public string ReactionTargetModifierMergeKey { get; private set; }

        public override IModifier CreateModifier(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, ResultBase parentResult)
        {
            return new EffectOnResultReactionModifier(Effect, EffectPriority, TriggerStyle, ReactionTargetModifierMergeKey, Duration, DurationStyle, ModifierAlignment, originator, IsModifierVisible, ModifierIconKey, FriendlyName, Description, StatusText);
        }
    }

    public class EffectOnGivingHitModifierFactoryJsonConverter : JsonConverter<EffectOnResultReactionModifierFactory>
    {
        public override EffectOnResultReactionModifierFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            IEffect effect = null;
            int duration = 0;
            DurationStyle? durationStyle = null;
            ModifierAlignment? modifierAlignment = null;
            Priority? effectPriority = null;
            EffectModifierTriggerStyle? triggerStyle = null;
            bool? isModifierVisible = null;
            string statusText = null;
            string modifierIconKey = null;
            string friendlyName = "Unnamed";
            string description = string.Empty;
            string reactionTargetModifierMergeKey = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Effect":
                        effect = JsonSerializer.Deserialize<IEffect>(ref reader, options);
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
                    case "StatusText":
                        statusText = reader.GetString();
                        reader.Read();
                        break;
                    case "EffectPriority":
                        effectPriority = JsonSerializer.Deserialize<Priority>(ref reader, options);
                        reader.Read();
                        break;
                    case "TriggerStyle":
                        triggerStyle = JsonSerializer.Deserialize<EffectModifierTriggerStyle>(ref reader, options);
                        reader.Read();
                        break;
                    case "ReactionTargetModifierMergeKey":
                        reactionTargetModifierMergeKey = reader.GetString();
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (effect == null) throw new JsonException("Expected 'Effect' property to be present.");
            if (isModifierVisible == null) throw new JsonException("Expected 'IsModifierVisible' property to be present.");
            if (durationStyle == null) throw new JsonException("Expected 'DurationStyle' property to be present.");
            if (modifierAlignment == null) throw new JsonException("Expected 'ModifierAlignment' property to be present.");
            if ((modifierIconKey == null) && (isModifierVisible.Value)) throw new JsonException("Expected 'ModifierIconKey' property to be present.");
            if (triggerStyle == null) throw new JsonException("Expected 'TriggerStyle' property to be present.");
            if (effectPriority == null) throw new JsonException("Expected 'EffectPriority' property to be present.");
            if ((reactionTargetModifierMergeKey == null) && (triggerStyle == EffectModifierTriggerStyle.OnOtherModifierRemovesItselfFromHost)) throw new JsonException("Expected 'ReactionTargetModifierMergeKey' property to be present.");

            return new EffectOnResultReactionModifierFactory(effect, effectPriority.Value, triggerStyle.Value, reactionTargetModifierMergeKey, duration, durationStyle.Value, modifierAlignment.Value, isModifierVisible.Value, modifierIconKey, friendlyName, description, statusText);
        }

        public override void Write(Utf8JsonWriter writer, EffectOnResultReactionModifierFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

