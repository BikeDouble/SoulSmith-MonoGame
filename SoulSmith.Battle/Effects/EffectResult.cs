using SoulSmith.Battle.Effects.Damage;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Modifiers;

namespace SoulSmith.Battle.Effects;
public partial class EffectResult
{
    public EffectResult(CombatTrigger trigger, IReadOnlyUnit sender, IReadOnlyUnit target)
    {
        TriggerApplied = trigger;
        Sender = sender;
        Target = target;
    }

    public EffectResult(int effectiveDamage, DamageType damageType, IReadOnlyUnit sender, IReadOnlyUnit target)
    {
        EffectiveDamage = effectiveDamage;
        DamageType = damageType;
        Sender = sender;
        Target = target;
    }

    public EffectResult(int effectiveHealing, IReadOnlyUnit sender, IReadOnlyUnit target)
    {
        EffectiveHealing = effectiveHealing;
        Sender = sender;
        Target = target;
    }

    public EffectResult(IReadOnlyModifier modifierApplied, IReadOnlyUnit sender, IReadOnlyUnit target)
    {
        ModifierApplied = modifierApplied;
        Sender = sender;
        Target = target;
    }

    public readonly int EffectiveDamage = 0;
    public readonly DamageType DamageType;
    public readonly int EffectiveHealing = 0;
    public readonly IReadOnlyUnit Sender;
    public readonly IReadOnlyUnit Target;
    public readonly IReadOnlyModifier ModifierApplied = null;
    public readonly CombatTrigger TriggerApplied = CombatTrigger.None;
}
