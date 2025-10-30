using SoulSmith.Battle.Effects;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;
using SoulSmith.UnitStats;
using SoulSmith.Localization;

namespace SoulSmith.Battle.Modifiers.Stat
{
    public class StaticStatModifier : Modifier, IReadOnlyStatModifier
    {
        public StaticStatModifier(
            StatType statType,
            StatModStyle modType,
            float modAmount,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            IEffectOriginator originator,
            bool isVisible,
            string iconKey,
            string friendlyName,
            string mergeKey)
            : base(duration, durationStyle, alignment, originator, isVisible, iconKey, friendlyName, "", "", mergeKey)
        {
            StatType = statType;
            ModStyle = modType;
            ModAmount = modAmount;
            UpdateDescription();
        }


        public override StatModifier? GetStatModifier()
        {
            return new StatModifier(StatType, ModStyle, ModAmount);
        }

        private string GetStatusTextInternal()
        {
            string ret;

            switch (ModStyle)
            {
                case StatModStyle.AdditivePercent:
                case StatModStyle.MultiplicativePercent:
                    ret = ((int)ModAmount).ToString() + "%";
                    break;
                default:
                    ret = ((int)ModAmount).ToString();
                    break;
            }

            return ret;
        }

        private void UpdateDescription()
        {
            string localizedStringKeu = StatType switch
            {
                StatType.Attack => ModStyle switch
                {
                    StatModStyle.Flat => ModAmount >= 0 ? "Modifiers/Descriptions/Generic/AttackUpFlat" : "Modifiers/Descriptions/Generic/AttackDownFlat",
                    StatModStyle.AdditivePercent => ModAmount >= 0 ? "Modifiers/Descriptions/Generic/AttackUpAdditive" : "Modifiers/Descriptions/Generic/AttackDownAdditive",
                    StatModStyle.MultiplicativePercent => ModAmount >= 0 ? "Modifiers/Descriptions/Generic/AttackUpMultiplicative" : "Modifiers/Descriptions/Generic/AttackDownMultiplicative",
                    _ => "Modifier_Description_Default"
                },
                StatType.Defense => ModStyle switch
                {
                    StatModStyle.Flat => ModAmount >= 0 ? "Modifiers/Descriptions/Generic/DefenseUpFlat" : "Modifiers/Descriptions/Generic/DefenseDownFlat",
                    StatModStyle.AdditivePercent => ModAmount >= 0 ? "Modifiers/Descriptions/Generic/DefenseUpAdditive" : "Modifiers/Descriptions/Generic/DefenseDownAdditive",
                    StatModStyle.MultiplicativePercent => ModAmount >= 0 ? "Modifiers/Descriptions/Generic/DefenseUpMultiplicative" : "Modifiers/Descriptions/Generic/DefenseDownMultiplicative",
                    _ => "Modifier_Description_Default"
                }
            };

            string localizedString = LocalizationManager.Instance.GetLocalizedString(localizedStringKeu, new Dictionary<string, string>
            {
                { "ModAmount", ModAmount.ToString() },
            });

            Description = localizedString;
        }

        protected override bool MergeInternal(IModifier other)
        {
            if (!(other is StaticStatModifier otherStatModifier)) return false;
            if (otherStatModifier.StatType != StatType) return false;
            if (otherStatModifier.ModStyle != ModStyle) return false;

            ModAmount = StatTypeHelper.CombineModifiers(ModAmount, otherStatModifier.ModAmount, ModStyle);
            UpdateDescription();

            return true;
        }

        public StatType StatType { get; private set; }
        public StatModStyle ModStyle { get; private set; }
        public override string StatusText { get { return GetStatusTextInternal(); } }
        public float ModAmount { get; private set; }
    }
}
