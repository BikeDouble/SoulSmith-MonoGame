using SoulSmith.UnitStats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Templates;

public class UnitTemplateJsonConverter : JsonConverter<UnitTemplate>
{
    public override UnitTemplate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of an object");
        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException("Expected property name");

        string friendlyName = string.Empty;
        string spriteName = string.Empty;
        int maxHealth = -1;
        int curHealth = -1;
        int curDecay = 0;
        int attack = -1;
        int defense = -1;
        int decayRate = StatConstants.STANDARDDECAYRATE;
        int timeOnBoard = -1;
        ReadOnlyCollection<string> moveSet = null;
        EmotionTag.EmotionTag emotionTag = EmotionTag.EmotionTag.Typeless;

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
            reader.Read();
            switch (propertyName)
            {
                case "FriendlyName":
                    friendlyName = reader.GetString();
                    break;
                case "SpriteName":
                    spriteName = reader.GetString();
                    break;
                case "MaxHealth":
                    maxHealth = reader.GetInt32();
                    break;
                case "CurHealth":
                    curHealth = reader.GetInt32();
                    break;
                case "CurDecay":
                    curDecay = reader.GetInt32();
                    break;
                case "DecayRate":
                    decayRate = reader.GetInt32();
                    break;
                case "Attack":
                    attack = reader.GetInt32();
                    break;
                case "Defense":
                    defense = reader.GetInt32();
                    break;
                case "Emotion":
                    emotionTag = JsonSerializer.Deserialize<EmotionTag.EmotionTag>(ref reader, options);
                    break;
                case "TimeOnBoard":
                    timeOnBoard = reader.GetInt32();
                    break;
                case "MoveSet":
                    string[] moveSetArray = JsonSerializer.Deserialize<string[]>(ref reader, options);
                    moveSet = moveSetArray.ToList().AsReadOnly();
                    break;
                default:
                    throw new JsonException($"Unknown property: {propertyName}");
            }
            reader.Read();
        }

        if (maxHealth <= 0)
            throw new JsonException($"No max health specified for unit: {friendlyName}");
        if (attack < 0)
            throw new JsonException($"No attack specified for unit: {friendlyName}");
        if (defense < 0)
            throw new JsonException($"No defense specified for unit: {friendlyName}");
        if ((moveSet is null) || moveSet.Count < 1)
            throw new JsonException($"No moveset specified for unit: {friendlyName}");

        if (curHealth <= 0) curHealth = maxHealth;
        Dictionary<StatType, int> statsList = new Dictionary<StatType, int>();
        statsList.Add(StatType.MaxHealth, maxHealth);
        statsList.Add(StatType.Attack, attack);
        statsList.Add(StatType.Defense, defense);
        statsList.Add(StatType.DecayRate, decayRate);
        statsList.Add(StatType.CurHealth, curHealth);
        statsList.Add(StatType.CurDecay, curDecay);

        UnitTemplate value = new UnitTemplate(statsList, moveSet, emotionTag, timeOnBoard, spriteName, friendlyName);

        return value;
    }

    public override void Write(Utf8JsonWriter writer, UnitTemplate value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteEndObject();
        throw new NotImplementedException();
    }
}
