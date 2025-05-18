
using SoulSmithMoves;
using System;

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
