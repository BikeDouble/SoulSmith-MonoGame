
using SoulSmith.Battle;
using SoulSmith.Effect.Modifier;
using System;

namespace SoulSmith.Effect;
public partial class EffectResult 
{
    public int EffectiveDamage = 0;
    public DamageType DamageType;
    public int EffectiveHealing = 0;
    public IReadOnlyUnit Sender;
    public IReadOnlyUnit Target;
    public IReadOnlyModifier ModifierApplied = null;
    public EffectTrigger TriggerApplied = EffectTrigger.None;
}
