using SoulSmith.Asset;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Visualization.Factory;
using SoulSmith.Battle.Modifiers.Effect;
using SoulSmith.Battle.Modifiers.Stat;
using SoulSmith.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Modifiers
{
    [JsonConverter(typeof(ModifierFactoryJsonConverter))]
    public class ModifierFactory : IDisposable
    {
        public ModifierFactory(int duration, DurationStyle durationStyle, ModifierAlignment modifierAlignment, bool isModifierVisible, string modifierIconKey, string friendlyName, string description, string mergeKey = null)
        {
            Duration = duration;
            DurationStyle = durationStyle;
            ModifierAlignment = modifierAlignment;
            IsModifierVisible = isModifierVisible;
            ModifierIconKey = modifierIconKey;
            FriendlyName = friendlyName ?? "Unnamed";
            Description = description ?? string.Empty;
            MergeKey = mergeKey;
        }

        public int Duration { get; private set; }
        public DurationStyle DurationStyle { get; private set; }
        public ModifierAlignment ModifierAlignment { get; private set; }
        public bool IsModifierVisible { get; private set; }
        public string ModifierIconKey { get; private set; }
        public string FriendlyName { get; private set; }
        public string Description { get; private set; }
        public string MergeKey { get; private set; }
        public virtual IModifier CreateModifier(IReadOnlyUnit sender, IReadOnlyUnit target, IReadOnlyCombat combat, IEffectOriginator originator, Result addEffectParentResult)
        {
            return null;
        }

        public void Dispose()
        {
            // Dispose logic if needed
        } 
    }

    public class ModifierFactoryJsonConverter : JsonConverter<ModifierFactory>
    {
        public override ModifierFactory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            if (reader.GetString() != "Type") throw new JsonException("Expected type of ModifierFactory");

            reader.Read();

            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected name of ModifierFactory type");

            string factoryType = reader.GetString();

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

            if (reader.GetString() != "Modifier") throw new JsonException("Expected ModifierFactory");

            reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object");

            ModifierFactory value = null;

            switch (factoryType)
            {
                case "StaticStat":
                    value = JsonSerializer.Deserialize<StaticStatModifierFactory>(ref reader, options);
                    reader.Read();
                    break;
                case "StaticStatBasedOnDamageDone":
                    value = JsonSerializer.Deserialize<StaticStatBasedOnDamageDoneModifierFactory>(ref reader, options);
                    reader.Read(); 
                    break;
                case "EffectOnResultReaction":
                    value = JsonSerializer.Deserialize<EffectOnResultReactionModifierFactory>(ref reader, options);
                    reader.Read();
                    break;
                case "RampageRefund":
                    value = JsonSerializer.Deserialize<RampageRefundModifierFactory>(ref reader, options);
                    reader.Read();
                    break;
                default:
                    throw new JsonException($"Unexpected type {factoryType}. Type is either misspelled or does not exist.");
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, ModifierFactory value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
