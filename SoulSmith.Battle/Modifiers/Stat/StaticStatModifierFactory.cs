using SoulSmith.Asset;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers;
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

namespace SoulSmith.Battle.Modifiers.Stat
{
    [JsonConverter(typeof(StaticStatModifierFactoryJsonConverter))]
    public class StaticStatModifierFactory : ModifierFactory
    {
        public StaticStatModifierFactory(
            StatType statType,
            int flatMod,
            double additiveMod,
            double multiplicativeMod,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            bool isVisible,
            DrawableResourceKey iconKey,
            string friendlyName,
            string description)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description)
        {
            StatType = statType;
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
        }

        public StatType StatType { get; private set; }
        public int FlatMod { get; private set; }
        public double AdditiveMod { get; private set; }
        public double MultiplicativeMod { get; private set; }

        public override IModifier CreateModifier()
        {
            return new StaticStatModifier(StatType, FlatMod, AdditiveMod, MultiplicativeMod, Duration, DurationStyle, ModifierAlignment, IsModifierVisible, ModifierIconKey, FriendlyName, Description);
        }
    }

    public class StaticStatModifierFactoryJsonConverter : JsonConverter<StaticStatModifierFactory>
    {
        public override StaticStatModifierFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            StatType statType = StatType.None;
            int flatMod = 0;
            double additiveMod = 0.0d;
            double multiplicativeMod = 1.0d;
            int duration = 0;
            DurationStyle? durationStyle = null;
            ModifierAlignment? modifierAlignment = null;
            bool? isModifierVisible = null;
            DrawableResourceKey modifierIconKey = null;
            string friendlyName = "Unnamed";
            string description = string.Empty;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
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
                        modifierIconKey = JsonSerializer.Deserialize<DrawableResourceKey>(ref reader, options);
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

            if (statType == StatType.None) throw new JsonException("Expected 'StatType' property to be present.");
            if ((flatMod == 0) && (additiveMod == 0d) && (multiplicativeMod == 1d)) throw new JsonException("Expected at least one of 'FlatMod', 'AdditiveMod', or 'MultiplicativeMod' to be non-default.");
            if (isModifierVisible == null) throw new JsonException("Expected 'IsModifierVisible' property to be present.");
            if (durationStyle == null) throw new JsonException("Expected 'DurationStyle' property to be present.");
            if (modifierAlignment == null) throw new JsonException("Expected 'ModifierAlignment' property to be present.");
            if ((modifierIconKey == null) && (isModifierVisible.Value)) throw new JsonException("Expected 'ModifierIconKey' property to be present.");
            
            return new StaticStatModifierFactory(statType, flatMod, additiveMod, multiplicativeMod, duration, durationStyle.Value, modifierAlignment.Value, isModifierVisible.Value, modifierIconKey, friendlyName, description);
        }

        public override void Write(Utf8JsonWriter writer, StaticStatModifierFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

