using SoulSmithMoves;

using System;
using System.Collections.Generic;

public partial class EffectRequest 
{
    public EffectRequest(Unit sender, Unit target)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ChildEffects = null;
    }

    public EffectRequest(Unit sender, Unit target, EffectTrigger trigger, List<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        Modifier = null;
        Trigger = trigger;
        ChildEffects = childEffects;
    }

    public EffectRequest(Unit sender, Unit target, DamageType damageType, int rawDamage, bool gainDecay = true, List<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = rawDamage;
        RawHealing = 0;
        DamageType = damageType;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects;
        GainDecay = gainDecay;
    }

    public EffectRequest(Unit sender, Unit target, int rawHealing, List<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = rawHealing;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects;
        GainDecay = false;
    }

    public EffectRequest(Unit sender, Unit target, Modifier modifier, List<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        Modifier = modifier;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects;
    }

    public Unit Sender { get; }
    public Unit Target { get; }
    public int RawDamage { get; }
    public DamageType DamageType { get; }
    public int RawHealing { get; }
    public bool GainDecay { get; }
    public Modifier Modifier { get; }
    public EffectTrigger Trigger { get; }
    public List<Effect> ChildEffects { get; }
}
