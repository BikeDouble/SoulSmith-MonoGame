using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Text.Json;
using SoulSmith.Battle.Effect;
using SoulSmith.Collections;
using SoulSmith.Core;
using SoulSmith.Asset;

namespace SoulSmith.Battle.Move
{
    [JsonConverter(typeof(MoveJsonConverter))]
    public class Move : IAsset
    {
        public Move(string friendlyName, string description, MoveTargetingStyle targetingStyle, EmotionTag.EmotionTag emotionTag, IList<IEffect> effects)
        {
            FriendlyName = friendlyName;
            Description = description;
            Effects = new ReadOnlyCollection<IEffect>(effects);
            TargetingStyle = targetingStyle;
            EmotionTag = emotionTag;
        }

        public ReadOnlyCollection<IEffect> Effects { get; }
        public EmotionTag.EmotionTag EmotionTag { get; }
        public MoveTargetingStyle TargetingStyle { get; }
        public string FriendlyName { get; }
        public string Description { get; }

        public void Dispose()
        {
            foreach (IEffect effect in Effects)
            {
                effect.Dispose();
            }
        }

        public static IEnumerable<Move> GenerateMoveList(IReadOnlySoulSmithWeightedList<string> possibleMovesPaths, int maxCount) //TODO move to asset
        {
            if (possibleMovesPaths.Count < maxCount) maxCount = possibleMovesPaths.Count;

            List<string> movePaths = possibleMovesPaths.NextMultiple(maxCount);

            List<Move> moves = new List<Move>(movePaths.Count);

            for (int i = 0; i < movePaths.Count; i++)
            {
                string fullMoveKey = "Moves/" + movePaths[i];

                Move curMove = (Move)AssetManager.Instance.GetMove<Move>(fullMoveKey);

                if (curMove == null) throw new ArgumentNullException(nameof(curMove));

                moves.Add(curMove);

            }

            return moves;
        }
    }

    public class MoveJsonConverter : JsonConverter<Move>
    {
        public override Move Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            string name = string.Empty;
            string localizationKey = string.Empty;
            Dictionary<string, string> localizationVariables = null;
            string description = "Localization keys not yet implemented.";
            IEffect[] effects = null;
            EmotionTag.EmotionTag emotionTag = EmotionTag.EmotionTag.Typeless;
            MoveTargetingStyle targetingStyle = MoveTargetingStyle.None;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Name":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        name = reader.GetString();
                        reader.Read();
                        break;
                    case "LocalizationKey":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        localizationKey = reader.GetString();
                        reader.Read();
                        break;
                    case "LocalizationVariables":
                        var optionsLocVars = new JsonSerializerOptions();
                        optionsLocVars.Converters.Add(new LocalizationVariablesJsonConverter());
                        localizationVariables = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, optionsLocVars);
                        reader.Read();
                        break;
                    case "EmotionTag":
                        emotionTag = JsonSerializer.Deserialize<EmotionTag.EmotionTag>(ref reader, options);
                        reader.Read();
                        break;
                    case "Effects":
                        effects = JsonSerializer.Deserialize<IEffect[]>(ref reader, options);
                        reader.Read();
                        break;
                    case "TargetingStyle":
                        targetingStyle = JsonSerializer.Deserialize<MoveTargetingStyle>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Read();

            return new Move(name, description, targetingStyle, emotionTag, effects);
        }

        public override void Write(Utf8JsonWriter writer, Move value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
