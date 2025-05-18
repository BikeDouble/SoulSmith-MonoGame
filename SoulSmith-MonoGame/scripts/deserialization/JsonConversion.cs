using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using SoulSmithStats;
using SoulSmithEmotions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using KaimiraGames;
using SoulSmith.Drawing;

namespace SoulSmithJsonConversion;

public class StatTypeJsonConverter : System.Text.Json.Serialization.JsonConverter<StatType>
{
    public override StatType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected to decode a string");
        string text = reader.GetString() ?? throw new JsonException("String cannot be null");
        return SoulSmithStats.StatTypeHelper.StringToStatType(text);
    }

    public override void Write(Utf8JsonWriter writer, StatType value, JsonSerializerOptions options)
    {
        string text = SoulSmithStats.StatTypeHelper.StatTypeToString(value);
        writer.WriteStringValue(text);
    }
}

public class EmotionTagJsonConverter : System.Text.Json.Serialization.JsonConverter<EmotionTag>
{
    public override EmotionTag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            int intTag = reader.GetInt32();
            return (EmotionTag)intTag;
        }
        else
            throw new JsonException("Expected to decode a number");
    }

    public override void Write(Utf8JsonWriter writer, EmotionTag value, JsonSerializerOptions options)
    {
        int intTag = (int)value;
        writer.WriteNumberValue(intTag);
    }
}

public class Vector2JsonConverter : System.Text.Json.Serialization.JsonConverter<Microsoft.Xna.Framework.Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of an object");
        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
            throw new JsonException("Expected property name");

        float x = 0;
        float y = 0;

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
            reader.Read();
            switch (propertyName)
            {
                case "X":
                    x = reader.GetSingle();
                    break;
                case "Y":
                    y = reader.GetSingle();
                    break;
                default:
                    throw new JsonException($"Unknown property: {propertyName}");
            }
            reader.Read();
        }
        Vector2 vector2 = new Vector2(x, y);
        return vector2;
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteEndObject();
    }
}

public class ColorJsonConverter : System.Text.Json.Serialization.JsonConverter<Microsoft.Xna.Framework.Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of an array");
        reader.Read();
        int[] color = { 255, 255, 255, 255 };
        int i = 0;

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            if (i >= 4)
                throw new JsonException("Expected maximum of 4 values");
            color[i] = reader.GetInt32();
            i++;
            reader.Read();
        }

        return new Color(color[0], color[1], color[2], color[3]);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.R);
        writer.WriteNumberValue(value.G);
        writer.WriteNumberValue(value.B);
        writer.WriteNumberValue(value.A);
        writer.WriteEndArray();
    }
}

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
        int decayRate = SoulSmithStats.StatConstants.STANDARDDECAYRATE;
        int timeOnBoard = -1;
        ReadOnlyCollection<string> moveSet = null;
        EmotionTag emotionTag = EmotionTag.Typeless;

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
                    emotionTag = JsonSerializer.Deserialize<EmotionTag>(ref reader, options);
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

public class SoulSmithWeightedListJsonConverter<T> : JsonConverter<SoulSmithWeightedList<T>>
{
    public override SoulSmithWeightedList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of an array");
        var serializationOptionsWithConverter = new JsonSerializerOptions();
        serializationOptionsWithConverter.Converters.Add(new WeightedListItemJsonConverter<T>());
        WeightedListItem<T>[] items = JsonSerializer.Deserialize<WeightedListItem<T>[]>(ref reader, serializationOptionsWithConverter);
        return new SoulSmithWeightedList<T>(items, Rand.Random);
    }

    public override void Write(Utf8JsonWriter writer, SoulSmithWeightedList<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (T item in value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Item");
            JsonSerializer.Serialize<T>(writer, item);
            writer.WriteNumber("Weight", value.GetWeightOf(item));
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}

public class WeightedListItemJsonConverter<T> : JsonConverter<WeightedListItem<T>>
{
    public override WeightedListItem<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of an object");
        reader.Read();

        int weight = -1;
        bool itemFound = false;
        T item = default(T);
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
            reader.Read();
            switch (propertyName)
            {
                case "Item":
                    item = JsonSerializer.Deserialize<T>(ref reader, options);
                    itemFound = true;
                    break;
                case "Weight":
                    weight = reader.GetInt32();
                    break;
            }
            reader.Read();
        }
        if (!itemFound)
            throw new JsonException("No item specified");
        if (weight == -1)
            throw new JsonException("No weight specified");
        WeightedListItem<T> value = new WeightedListItem<T>(item, weight);
        return value;
    }

    public override void Write(Utf8JsonWriter writer, WeightedListItem<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Item");
        JsonSerializer.Serialize(writer, value._item);
        writer.WriteNumber("Weight", value._weight);
        writer.WriteEndObject();
    }
}