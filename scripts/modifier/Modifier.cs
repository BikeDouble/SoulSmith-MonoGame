

using SoulSmithStats;
using SoulSmithMoves;
using System;

public partial class Modifier 
{
    private Unit _host;
    private Unit _applier;
    private bool _permanent = true;
    private int _roundsDuration;

    public Modifier(int duration = -1)
    {
        if (duration >= 0)
        {
            _permanent = false;
            _roundsDuration = duration;
        }
    }

    public virtual void ApplyTo(Unit host, Unit applier = null)
    {
        _applier = applier;
        _host = host;
    }

    public virtual void OnApplied()
    {

    }

    public event EventHandler<RemoveModifierEventArgs> RemoveModifierEventHandler;

    public virtual void Remove()
    {
        RemoveModifierEventArgs e = new RemoveModifierEventArgs();
        e.Modifier = this;

        RemoveModifierEventHandler(this, e);
        OnRemoved();
    }

    public virtual void OnRemoved()
    {
        
    }

    public event EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler;

    public void EnqueueEffectInput(EffectInput effectInput)
    {
        EnqueueEffectInputEventArgs e = new();
        e.EffectInput = effectInput;

        EnqueueEffectInputEventHandler(this, e);
    }

    //
    //Triggers
    //

    public virtual void OnHitting(Unit target)
    {

    }

    public virtual void OnBeingHit(Unit hitter)
    {

    }

    public virtual void OnMoveBegin(Unit target)
    {

    }

    public virtual void OnMoveEnd(Unit target)
    {

    }

    public virtual void OnTurnBegin(Unit self)
    {

    }

    public virtual void OnTurnEnd(Unit mover)
    {

    }

    public virtual void OnRoundBegin(Unit self)
    {

    }

    public virtual void OnRoundEnd(Unit self)
    {
        if (!_permanent) 
        {
            _roundsDuration--;
            if (_roundsDuration <= 0)
            {
                Remove();
            }
        }
    }

    public virtual void SetRoundsDuration(int duration)
    {
        _permanent = false;
        _roundsDuration = duration;
    }

    public virtual StatModifier GetStatModifier(StatType stat)
    {
        return new StatModifier();
    }


}

public class RemoveModifierEventArgs : EventArgs
{
    public Modifier Modifier;
}

public class EnqueueEffectInputEventArgs : EventArgs
{
    public EffectInput EffectInput;
}