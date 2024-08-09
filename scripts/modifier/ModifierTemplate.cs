using SoulSmithMoves;
using SoulSmithStats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmithModifiers;
public class ModifierTemplate
{
    public readonly EffectTrigger DecrementTrigger = EffectTrigger.None;
    public readonly Func<ModifierDelegateArgs, ModifierCommands> ProcessEffectResultDelegate;
    public readonly Func<ModifierDelegateArgs, ModifierCommands> ApplyDelegate;
    public readonly Func<ModifierDelegateArgs, ModifierCommands> RemoveDelegate;
    public readonly Func<ModifierDelegateArgs, StatModifier> GetStatModifierDelegate;
    public readonly string ModifierID;
    public readonly string IconName;
    public readonly EffectTemplate Effect;

    public ModifierTemplate(
        EffectTrigger decrementTrigger = EffectTrigger.None,
        string modifierID = null,
        string iconName = null,
        EffectTemplate effect = null,
        Func<ModifierDelegateArgs, ModifierCommands> processEffectResult = null,
        Func<ModifierDelegateArgs, ModifierCommands> apply = null,
        Func<ModifierDelegateArgs, ModifierCommands> remove = null,
        Func<ModifierDelegateArgs, StatModifier> getStatModifierDelegate = null)
    {
        DecrementTrigger = decrementTrigger;
        ProcessEffectResultDelegate = processEffectResult;
        ApplyDelegate = apply;
        RemoveDelegate = remove;
        IconName = iconName;
        ModifierID = modifierID;
        GetStatModifierDelegate = getStatModifierDelegate;
        Effect = effect;
    }

    public Modifier InstantiateAndApply(
        IAssetLoadOnly loader,
        IReadOnlyUnit host,
        IReadOnlyUnit applier,
        IDictionary<ModifierFloatArgType, float> floats)
    {
        CanvasItem icon = loader.GetSprite(IconName);

        Modifier modifier = new Modifier(
            host,
            applier,
            ModifierID,
            loader.LoadEffectFromTemplate(Effect),
            DecrementTrigger,
            (CanvasItem)icon?.DeepClone(),
            ProcessEffectResultDelegate,
            ApplyDelegate,
            RemoveDelegate,
            GetStatModifierDelegate,
            floats);

        return modifier;
    }


    public static ModifierTemplate BasicStaticStatModifier(
        StatType stat,
        string modifierID = null,
        string iconName = null)
    {
        Func<ModifierDelegateArgs, StatModifier> getStatModDelegate;

        switch (stat)
        {
            case StatType.Attack:
                getStatModDelegate = GetStaticAttackModifierFunc; break;
            default:
                getStatModDelegate = null; break;
        }

        return new ModifierTemplate(
            EffectTrigger.OnRoundEnd,
            modifierID,
            iconName,
            null,
            null,
            null,
            null,
            getStatModDelegate);
    }

    public static Func<ModifierDelegateArgs, StatModifier> GetStaticAttackModifierFunc =
        GenerateGetStaticStatModifierFunc(StatType.Attack);

    private static Func<ModifierDelegateArgs, StatModifier> GenerateGetStaticStatModifierFunc(StatType stat)
    {
        return (args) =>
        {
            int flat = 0;
            float add = 0;
            float mult = 1;

            var floats = args.Floats;

            if (floats == null) return new();

            if (floats.ContainsKey(ModifierFloatArgType.FlatMod)) flat = (int)floats[ModifierFloatArgType.FlatMod];

            if (floats.ContainsKey(ModifierFloatArgType.AddMod)) add = floats[ModifierFloatArgType.AddMod];

            if (floats.ContainsKey(ModifierFloatArgType.MultMod)) mult = floats[ModifierFloatArgType.MultMod];

            return new StatModifier(
                stat,
                flat,
                add,
                mult);
        };
    }

    public static StatType FloatToStatType(float val)
    {
        return (StatType)val;
    }

    /*public static ModifierTemplate SpecialArgFlatEssenceDamageOnHitModifier(
        string modifierID = null,
        string iconName = null,
        string effectVisName = null,
        float effectVisDelay = 0)
    {
        EffectTemplate effect = EffectTemplate.SpecialArgFlatEssenceDamage(
            null,
            effectVisName,
            effectVisDelay);

        return new ModifierTemplate(
            EffectTrigger.OnRoundEnd,
            modifierID,
            iconName,
            effect,
            SimpleEffectOnHitFunc,
            null,
            null);
    }*/

    public static ModifierTemplate SpecialArgStatBasedEssenceDamageOnHitModifier(
        string modifierID = null,
        string iconName = null,
        string effectVisName = null,
        float effectVisDelay = 0)
    {
        EffectTemplate effect = EffectTemplate.SpecialArgStatBasedEssenceDamage(
            null,
            effectVisName,
            effectVisDelay);

        return new ModifierTemplate(
            EffectTrigger.OnRoundEnd,
            modifierID,
            iconName,
            effect,
            SpecialArgStatTypeAndStatPercentEffectOnHit,
            null,
            null);
    }

    private static Func<ModifierDelegateArgs, ModifierCommands> SimpleEffectOnHitFunc = (args) =>
    {
        ModifierCommands commands = new();

        if (IsOnHit(args))
        {
            commands.SendEffect = true;
            commands.Target = args.EffectResult.Target;
        }

        return commands;
    };

    private static Func<ModifierDelegateArgs, ModifierCommands> SpecialArgStatTypeAndStatPercentEffectOnHit = (args) =>
    {
        ModifierCommands commands = new();

        if (IsOnHit(args))
        {
            commands.SendEffect = true;
            commands.Target = args.EffectResult.Target;

            List<float> specialArgs = new();
            specialArgs.Add(args.Floats[ModifierFloatArgType.StatType]);
            specialArgs.Add(args.Floats[ModifierFloatArgType.StatPercent]);

            commands.EffectSpecialArgs = specialArgs.AsReadOnly();
        }

        return commands;
    };
    
    private static bool IsOnHit(ModifierDelegateArgs args)
    {
        return (args.EffectResult.Sender == args.Host)
            && (args.EffectResult.DamageType == DamageType.Hit)
            && (args.EffectResult.EffectiveDamage > 0);
    }
}

public struct ModifierTemplateWithArgs
{
    public ModifierTemplateWithArgs(ModifierTemplate template, IDictionary<ModifierFloatArgType, float> args)
    {
        Template = template;
        Args = new ReadOnlyDictionary<ModifierFloatArgType, float>(args);
    }

    public ModifierTemplate Template { get; }
    public ReadOnlyDictionary<ModifierFloatArgType, float> Args { get; }
}

