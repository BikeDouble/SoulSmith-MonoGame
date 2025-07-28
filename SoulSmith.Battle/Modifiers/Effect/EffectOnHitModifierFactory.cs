using SoulSmith.Asset;
using SoulSmith.Battle.Effects;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers.Effect
{
    [JsonConverter(typeof(FollowUpEffectModifierFactoryJsonConverter))]
    public class EffectOnHitModifierFactory : ModifierFactory
    {
        public EffectOnHitModifierFactory(
            IEffect effect,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            bool isVisible,
            string iconKey,
            string friendlyName,
            string description)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description)
        {
            Effect = effect;
        }

        public IEffect Effect { get; private set; }

        public override IModifier CreateModifier()
        {
            return new EffectOnHitModifier(Effect, Duration, DurationStyle, ModifierAlignment, IsModifierVisible, ModifierIconKey, FriendlyName, Description);
        }
    }

    public class FollowUpEffectModifierFactoryJsonConverter : JsonConverter<EffectOnHitModifierFactory>
    {
        public override EffectOnHitModifierFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            IEffect effect = null;
            int duration = 0;
            DurationStyle? durationStyle = null;
            ModifierAlignment? modifierAlignment = null;
            bool? isModifierVisible = null;
            string modifierIconKey = null;
            string friendlyName = "Unnamed";
            string description = string.Empty;

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

            return new EffectOnHitModifierFactory(effect, duration, durationStyle.Value, modifierAlignment.Value, isModifierVisible.Value, modifierIconKey, friendlyName, description);
        }

        public override void Write(Utf8JsonWriter writer, EffectOnHitModifierFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

