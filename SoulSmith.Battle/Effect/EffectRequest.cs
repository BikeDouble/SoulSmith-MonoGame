using SoulSmith.Battle.Effect.Damage;
using SoulSmith.Battle.Effect.Trigger;
using SoulSmith.Battle.Modifier;

namespace SoulSmith.Battle.Effect;
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
        ImmediateAfterEffects = null;
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, EffectTrigger trigger, IEnumerable<IEffect> immediateAfterEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        Modifier = null;
        Trigger = trigger;
        ImmediateAfterEffects = immediateAfterEffects?.ToList();
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, DamageType damageType, int rawDamage, bool gainDecay = true, IEnumerable<IEffect> immediateAfterEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = rawDamage;
        RawHealing = 0;
        DamageType = damageType;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ImmediateAfterEffects = immediateAfterEffects?.ToList();
        GainDecay = gainDecay;
    }

    public EffectRequest(IReadOnlyUnit sender, IReadOnlyUnit target, int rawHealing, IEnumerable<IEffect> immediateAfterEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = rawHealing;
        Modifier = null;
        Trigger = EffectTrigger.None;
        ImmediateAfterEffects = immediateAfterEffects?.ToList();
        GainDecay = false;
    }

    public EffectRequest(
        IReadOnlyUnit sender,
        IReadOnlyUnit target,
        IModifier modifier,
        IEnumerable<IEffect> immediateAfterEffects = null)
    {
        Sender = sender;
        Target = target;
        RawDamage = 0;
        RawHealing = 0;
        Modifier = modifier;
        Trigger = EffectTrigger.None;
        ImmediateAfterEffects = immediateAfterEffects?.ToList();
    }

    public IReadOnlyUnit Sender { get; set; }
    public IReadOnlyUnit Target { get; set; }
    public int RawDamage { get; set; }
    public DamageType DamageType { get; set; }
    public int RawHealing { get; set; }
    public bool GainDecay { get; set; }
    public IModifier Modifier { get; set; }
    public EffectTrigger Trigger { get; set; }
    public List<IEffect> ImmediateAfterEffects { get; set; }
}
