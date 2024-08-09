using SoulSmithMoves;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmithModifiers;

public partial class EffectRequest 
{
    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        ModifierTemplate = null;
        ModifierArgs = null;
        Trigger = EffectTrigger.None;
        ChildEffects = null;
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, EffectTrigger trigger, IEnumerable<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        ModifierTemplate = null;
        ModifierArgs = null;
        Trigger = trigger;
        ChildEffects = childEffects?.ToList().AsReadOnly();
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, DamageType damageType, int rawDamage, bool gainDecay = true, IEnumerable<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = rawDamage;
        RawHealing = 0;
        DamageType = damageType;
        ModifierTemplate = null;
        ModifierArgs = null;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects?.ToList().AsReadOnly();
        GainDecay = gainDecay;
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, int rawHealing, IEnumerable<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = rawHealing;
        ModifierTemplate = null;
        ModifierArgs = null;
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects?.ToList().AsReadOnly();
        GainDecay = false;
    }

    public EffectRequest(
        IReadOnlyUnit sender,
        IReadOnlyUnit target,
        ModifierTemplate modifierTemplate,
        IDictionary<ModifierFloatArgType, float> modifierArgs = null,
        IEnumerable<Effect> childEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        ModifierTemplate = modifierTemplate;
        ModifierArgs = new(modifierArgs);
        Trigger = EffectTrigger.None;
        ChildEffects = childEffects?.ToList().AsReadOnly();
    }

    public IReadOnlyUnit Sender { get; }
    public IReadOnlyUnit Target { get; }
    public int RawDamage { get; }
    public DamageType DamageType { get; }
    public int RawHealing { get; }
    public bool GainDecay { get; }
    public ModifierTemplate ModifierTemplate { get; }
    public ReadOnlyDictionary<ModifierFloatArgType, float> ModifierArgs { get; }
    public EffectTrigger Trigger { get; }
    public ReadOnlyCollection<Effect> ChildEffects { get; }
}
