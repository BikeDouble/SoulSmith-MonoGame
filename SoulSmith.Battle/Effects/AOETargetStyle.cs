using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Battle.Effects;

[JsonConverter(typeof(AOETargetStyleJsonConverter))]
public enum AOETargetStyle
{
    Adjacent,
    WholeTeam,
    WholeCombat
}

public class AOETargetStyleJsonConverter : JsonConverter<AOETargetStyle>
{
    public override AOETargetStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string value for {nameof(AOETargetStyle)}, but got {reader.TokenType}");
        }

        string value = reader.GetString().ToLowerInvariant();

        return value switch
        {
            "adjacent" => AOETargetStyle.Adjacent,
            "wholeteam" => AOETargetStyle.WholeTeam,
            "wholecombat" => AOETargetStyle.WholeCombat,
            _ => throw new JsonException($"Unknown {nameof(AOETargetStyle)} value: {value}")
        };
    }

    public override void Write(Utf8JsonWriter writer, AOETargetStyle value, JsonSerializerOptions options)
    {
        string stringValue = value switch
        {
            AOETargetStyle.Adjacent => "adjacent",
            AOETargetStyle.WholeTeam => "wholeteam",
            AOETargetStyle.WholeCombat => "wholecombat",
            _ => throw new JsonException($"Unknown {nameof(AOETargetStyle)} value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}


