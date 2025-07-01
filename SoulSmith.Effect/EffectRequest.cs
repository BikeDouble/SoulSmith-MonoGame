using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Battle;
using SoulSmith.Effect.Modifier;

namespace SoulSmith.Effect;
public class EffectRequest
{
    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ChildEffects = null;
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, EffectTrigger trigger, IEnumerable<IEffect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        Modifier = null;
        Trigger = trigger;
        ChildEffects = childEffects?.ToList();
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, DamageType damageType, int rawDamage, bool gainDecay = true, IEnumerable<IEffect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = rawDamage;
        RawHealing = 0;
        DamageType = damageType;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects?.ToList();
        GainDecay = gainDecay;
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, int rawHealing, IEnumerable<IEffect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = rawHealing;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects?.ToList();
        GainDecay = false;
    }

    public EffectRequest(
        IReadOnlyUnit sender,
        IReadOnlyUnit target,
        IModifier modifier,
        IEnumerable<IEffect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        Modifier = modifier;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects?.ToList();
    }

    public IReadOnlyUnit Sender { get; set; }
    public IReadOnlyUnit Target { get; set; }
    public int RawDamage { get; set; }
    public DamageType DamageType { get; set; }
    public int RawHealing { get; set; }
    public bool GainDecay { get; set; }
    public IModifier Modifier { get; set; }
    public EffectTrigger Trigger { get; set; }
    public List<IEffect> ChildEffects { get; set; }
}
