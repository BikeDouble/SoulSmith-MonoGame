

using SoulSmithStats;
using SoulSmithMoves;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SoulSmithModifiers;
public class Modifier : IReadOnlyModifier
{
    private IReadOnlyUnit _host;
    private IReadOnlyUnit _applier;
    private EffectTrigger _decrementTrigger = EffectTrigger.None;
    private Func<ModifierDelegateArgs, ModifierCommands> _processEffectResultDelegate;
    private Func<ModifierDelegateArgs, ModifierCommands> _applyDelegate;
    private Func<ModifierDelegateArgs, ModifierCommands> _removeDelegate;
    private Func<ModifierDelegateArgs, StatModifier> _getStatModifierDelegate;
    private IReadOnlyCanvasItem _displayIcon = null;
    private Dictionary<ModifierFloatArgType, float> _floats = null;
    private bool _removeAtEndOfTurn = false;
    private string _modifierID = string.Empty;
    private Effect _effect = null;

    public Modifier(
        IReadOnlyUnit host, 
        IReadOnlyUnit applier, 
        string modifierID,
        Effect effect,
        EffectTrigger decrementTrigger = EffectTrigger.None,
        IReadOnlyCanvasItem displayIcon = null,
        Func<ModifierDelegateArgs, ModifierCommands> processEffectResult = null,
        Func<ModifierDelegateArgs, ModifierCommands> apply = null,
        Func<ModifierDelegateArgs, ModifierCommands> remove = null,
        Func<ModifierDelegateArgs, StatModifier> getStatModifierDelegate = null,
        IDictionary<ModifierFloatArgType, float> floats = null)
    {
        _decrementTrigger = decrementTrigger;
        _processEffectResultDelegate = processEffectResult;
        _applyDelegate = apply;
        _removeDelegate = remove;
        _getStatModifierDelegate = getStatModifierDelegate;
        _displayIcon = displayIcon;
        _modifierID = modifierID;
        _effect = effect;

        if (floats != null) 
            _floats = new Dictionary<ModifierFloatArgType, float>(floats);

        Apply(host, applier);
    }

    private void Apply(IReadOnlyUnit host, IReadOnlyUnit applier = null)
    {
        _applier = applier;
        _host = host;
        ProcessCommandDelegate(_applyDelegate);
    }

    public event EventHandler<RemoveModifierEventArgs> RemoveModifierEventHandler;

    private void Remove()
    {
        ProcessCommandDelegate(_removeDelegate);

        RemoveModifierEventArgs e = new RemoveModifierEventArgs();
        e.Modifier = this;

        RemoveModifierEventHandler(this, e);
    }

    public event EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler;

    public void EnqueueEffectInput(EffectInput effectInput)
    {
        EnqueueEffectInputEventArgs e = new();
        e.EffectInput = effectInput;

        EnqueueEffectInputEventHandler(this, e);
    }

    public void ProcessEffectResult(EffectResult result)
    {
        if (_removeAtEndOfTurn && (result.TriggerApplied == EffectTrigger.OnTurnEnd))
            Remove();

        if ((_decrementTrigger != EffectTrigger.None) && (_decrementTrigger == result.TriggerApplied) && (result.Target == _host))
            DecrementDuration();

        ProcessCommandDelegate(_processEffectResultDelegate, result);
    }

    private void ProcessCommandDelegate(Func<ModifierDelegateArgs, ModifierCommands> func, EffectResult effectResult = null)
    {
        if (func != null)
            ProcessModifierCommands(func(DelegateArgs(effectResult)));
    }

    private StatModifier ProcessStatModifierDelegate(Func<ModifierDelegateArgs, StatModifier> func)
    {
        if (func == null) return new();

        return func(DelegateArgs());
    }

    private ModifierDelegateArgs DelegateArgs(EffectResult effectResult = null)
    {
        ModifierDelegateArgs args = new ModifierDelegateArgs();
        args.EffectResult = effectResult;
        args.Host = _host;
        args.Applier = _applier;
        args.Floats = new ReadOnlyDictionary<ModifierFloatArgType, float>(_floats);

        return args;
    }
    
    private void ProcessModifierCommands(ModifierCommands output)
    {
        if (output == null) return;

        if (output.SendEffect)
        {
            if (_effect == null)
            {
                Trace.TraceError("Modifier " + _modifierID + ": no effect to Input");
            }
            else
            {
                EffectInput input = new EffectInput(_effect, _host, output.Target, output.EffectSpecialArgs);
                EnqueueEffectInput(input);
            }
        }

        _removeAtEndOfTurn = output.RemoveAtEndOfTurn;

        if (output.FloatChanges != null)
        {
            foreach (var change in output.FloatChanges)
            {
                _floats[change.Index] += change.Change;
            }
        }

    }

    private void DecrementDuration()
    {
        int duration = (int)_floats.GetValueOrDefault(ModifierFloatArgType.Duration);

        if (duration > 0) 
        {
            duration--;
            _floats[ModifierFloatArgType.Duration] = duration;

            if (duration <= 0)
            {
                Remove();
            }
        }
    }

    public virtual StatModifier GetStatModifier(StatType stat)
    {
        return ProcessStatModifierDelegate(_getStatModifierDelegate);
    }

    public IReadOnlyCanvasItem Icon { get { return _displayIcon; } }
}

public enum ModifierFloatArgType
{
    None,
    Duration,
    FlatMod,
    AddMod,
    MultMod,
    StatType,
    StatPercent
}

public class ModifierDelegateArgs
{
    public EffectResult EffectResult;
    public IReadOnlyUnit Host;
    public IReadOnlyUnit Applier;
    public ReadOnlyDictionary<ModifierFloatArgType, float> Floats;
}

public class ModifierCommands
{
    public bool SendEffect = false;
    public IReadOnlyUnit Target = null;
    public ReadOnlyCollection<float> EffectSpecialArgs = null;
    public bool RemoveImmediately = false;
    public bool RemoveAtEndOfTurn = false;
    public IEnumerable<(ModifierFloatArgType Index, float Change)> FloatChanges = null;
}

public class RemoveModifierEventArgs : EventArgs
{
    public Modifier Modifier;
}

public class EnqueueEffectInputEventArgs : EventArgs
{
    public EffectInput EffectInput;
}