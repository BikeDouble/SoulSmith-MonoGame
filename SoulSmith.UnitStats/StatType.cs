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
        public static float CombineAndApplyStyledModifiers(float value, IEnumerable<IStyledNumberModifier> modifiers)
        {
            return ApplyCombinedStyledModifier(value, CombineStyledModifiers(modifiers));
        }

        public static (float Flat, float AdditivePercent, float MultiplicativePercent) CombineStyledModifiers(IEnumerable<IStyledNumberModifier> modifiers)
        {
            if (modifiers.Count() == 0)
            {
                return (0, 0, 0);
            }

            float flat = 0;
            float additivePercent = 0f;
            float multiplicativePercent = 0f;

            foreach (IStyledNumberModifier modifier in modifiers)
            {
                switch (modifier.ModStyle)
                {
                    case StatModStyle.Flat:
                        flat = StatTypeHelper.CombineModifiers(flat, modifier.ModAmount, StatModStyle.Flat);
                        break;
                    case StatModStyle.AdditivePercent:
                        additivePercent = StatTypeHelper.CombineModifiers(additivePercent, modifier.ModAmount, StatModStyle.AdditivePercent);
                        break;
                    case StatModStyle.MultiplicativePercent:
                        multiplicativePercent = StatTypeHelper.CombineModifiers(multiplicativePercent, modifier.ModAmount, StatModStyle.MultiplicativePercent);
                        break;

                }
            }

            return (flat, additivePercent, multiplicativePercent);
        }

        public static float ApplyCombinedStyledModifier(float baseStat, (double Flat, double AdditivePercent, double MultiplicativePercent) combinedModifier)
        {
            // Flat modifier is applied immediately to the base stat.
            float retStat = baseStat + (int)combinedModifier.Flat;

            // Additive percent modifier is applied to the base stat after the flat modifier.
            float additiveAsDecimal = (float)((combinedModifier.AdditivePercent + 100) / 100);
            retStat = (int)(retStat * additiveAsDecimal);

            // Multiplicative percent modifier is applied to the base stat after the flat and additive percent modifiers.
            float multiplicativeAsDecimal = (float)((combinedModifier.MultiplicativePercent + 100) / 100);
            retStat = (int)(retStat * multiplicativeAsDecimal);

            return retStat;
        }

        public static float CombineModifiers(float a, float b, StatModStyle modStyle)
        {
            switch (modStyle)
            {
                case StatModStyle.Flat:
                    return a + b;
                case StatModStyle.AdditivePercent:
                    return a + b;
                case StatModStyle.MultiplicativePercent:
                    float aAsDecimal = (100 + a) / 100;
                    float bAsDecimal = (100 + b) / 100;
                    float resultAsDecimal = aAsDecimal * bAsDecimal;
                    return resultAsDecimal * 100 - 100; // Convert back to percentage
                default:
                    throw new ArgumentException("Invalid mod style");
            }
        }

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

    public readonly struct StatModifier : IStyledNumberModifier
    {
        public StatModifier(StatType stat, StatModStyle modType, float modAmount)
        {
            Stat = stat;
            ModStyle = modType;
            ModAmount = modAmount;
        }

        public StatModifier()
        {
            Stat = StatType.None;
            ModStyle = StatModStyle.Null;
            ModAmount = 0;
        }

        public StatType Stat { get; }
        public StatModStyle ModStyle { get; }
        public float ModAmount { get; }
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
