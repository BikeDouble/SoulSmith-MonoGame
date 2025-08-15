using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects
{
    [JsonConverter(typeof(PriorityJsonConverter))]
    public enum Priority
    {
        Move,
        ModifierRemovalDelayed,
        ModifierRemovalImmediate,
        Body,
        Reaction,
        ImmediateAfterEffect,
        EmotionCombatEntryEffect,
        NonMoveCombatTrigger
    }

    /// <summary>
    /// JSON converter for the Priority enum.
    /// </summary>
    public class PriorityJsonConverter : JsonConverter<Priority>
    {
        /// <summary>
        /// Reads and converts the JSON to a Priority enum value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted Priority value.</returns>
        public override Priority Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string enumString = reader.GetString().ToLower();

                switch (enumString)
                {
                    case "move":
                        return Priority.Move;
                    case "modifierremovaldelayed":
                        return Priority.ModifierRemovalDelayed;
                    case "modifierremovalimmediate":
                        return Priority.ModifierRemovalImmediate;
                    case "body":
                        return Priority.Body;
                    case "reaction":
                        return Priority.Reaction;
                    case "immediateaftereffect":
                        return Priority.ImmediateAfterEffect;
                    case "emotioncombatentryeffect":
                        return Priority.EmotionCombatEntryEffect;
                    case "nonmovecombattrigger":
                        return Priority.NonMoveCombatTrigger;
                    default:
                        throw new JsonException($"Unable to convert \"{enumString}\" to enum type {typeof(Priority)}.");
                }
            }
            
            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }

        /// <summary>
        /// Writes a Priority enum value as JSON.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, Priority value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
