using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.UnitStats
{
    public static class StatConstants
    {
        public const int STANDARDDECAYRATE = 100; //TODO move to UnitStats
    }

    public static class StatTypeHelper
    {
        public static StatType StringToStatType(string text)
        {
            string textLower = text.ToLower();

            switch (textLower)
            {
                case "attack":
                    return StatType.Attack;
                case "defense":
                    return StatType.Defense;
                case "maxhealth":
                    return StatType.MaxHealth;
                case "curhealth":
                    return StatType.CurHealth;
                case "curdecay":
                    return StatType.CurDecay;
                case "decayrate":
                    return StatType.DecayRate;
                default:
                    return StatType.None;
            }
        }

        public static string StatTypeToString(StatType type)
        {
            switch (type)
            {
                case StatType.Attack:
                    return "attack";
                case StatType.Defense:
                    return "defense";
                case StatType.MaxHealth:
                    return "maxhealth";
                case StatType.CurHealth:
                    return "curhealth";
                case StatType.CurDecay:
                    return "curdecay";
                case StatType.DecayRate:
                    return "decayrate";
                default:
                    return string.Empty;
            }
        }
    }

    [JsonConverter(typeof(StatTypeJsonConverter))]
    public enum StatType
    {
        None = 0,
        MaxHealth,
        CurHealth,
        Attack,
        Defense,
        CurDecay,
        DecayRate
    }

    public readonly struct StatModifier
    {
        public StatModifier(StatType stat, int flatMod = 0, double additiveMod = 0f, double multiplicativeMod = 1f)
        {
            Stat = stat;
            FlatMod = flatMod;
            AdditiveMod = additiveMod;
            MultiplicativeMod = multiplicativeMod;
        }

        public StatModifier()
        {
            Stat = StatType.None;
            FlatMod = 0;
            AdditiveMod = 0f;
            MultiplicativeMod = 1f;
        }

        public StatType Stat { get; }
        public int FlatMod { get; }
        public double AdditiveMod { get; }
        public double MultiplicativeMod { get; }
    }

    public class StatTypeJsonConverter : System.Text.Json.Serialization.JsonConverter<StatType>
    {
        public override StatType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected to decode a string");
            string text = reader.GetString() ?? throw new JsonException("String cannot be null");
            return StatTypeHelper.StringToStatType(text);
        }

        public override void Write(Utf8JsonWriter writer, StatType value, JsonSerializerOptions options)
        {
            string text = StatTypeHelper.StatTypeToString(value);
            writer.WriteStringValue(text);
        }
    }
}
