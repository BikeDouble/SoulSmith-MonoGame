using System.Collections.Generic;
using System;
using SoulSmithMoves;
using SoulSmithObjects;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

public partial class EffectQueue : CanvasItem
{
    public const double UNIVERSALMOVEEFFECTDELAY = UnitSprite.ATTACKANIMATIONDURATION / 2;

    private Queue<QueuedEffect> _queue;
    private Queue<QueuedEffect> _priorityQueue;
    private DropOutStack<(EffectRequest, EffectResult)> _effectHistory; //Effect history is pushed after effect is processed
    private DropOutStack<MoveInput> _moveHistory; //Move history is pushed after move is queued
    private bool _processingEnabled = true;
    private IReadOnlyUnit _lastMoveTarget;
    private Effect _moveBeginEffect = GameManager.InstantiateNakedEffect(EffectTemplate.Trigger(EffectTrigger.OnMoveBegin, EffectTargetingStyle.PredeterminedGlobalTrigger));
    private Effect _moveEndEffect = GameManager.InstantiateNakedEffect(EffectTemplate.Trigger(EffectTrigger.OnMoveEnd, EffectTargetingStyle.PredeterminedGlobalTrigger));
    private Effect _roundBeginEffect = GameManager.InstantiateNakedEffect(EffectTemplate.Trigger(EffectTrigger.OnRoundBegin, EffectTargetingStyle.PredeterminedGlobalTrigger));
    private Effect _roundEndEffect = GameManager.InstantiateNakedEffect(EffectTemplate.Trigger(EffectTrigger.OnRoundEnd, EffectTargetingStyle.PredeterminedGlobalTrigger));
    private Effect _turnBeginEffect = GameManager.InstantiateNakedEffect(EffectTemplate.Trigger(EffectTrigger.OnTurnBegin, EffectTargetingStyle.PredeterminedGlobalTrigger));
    private Effect _turnEndEffect = GameManager.InstantiateNakedEffect(EffectTemplate.Trigger(EffectTrigger.OnTurnEnd, EffectTargetingStyle.PredeterminedGlobalTrigger));
    private Effect _unitDeathEffect = GameManager.InstantiateNakedEffect(EffectTemplate.Trigger(EffectTrigger.OnUnitDeath, EffectTargetingStyle.PredeterminedGlobalTrigger));

    public readonly struct QueuedEffect
    {
        public QueuedEffect(EffectInput input, EffectResult parentEffectResult, double additionalDelay)
        {
            EffectInput = input;
            VisualizationListener = new EffectVisualizationListener(input, additionalDelay);
            ParentEffectResult = parentEffectResult;
        }

        public EffectInput EffectInput { get; }
        public EffectVisualizationListener VisualizationListener { get; }
        public EffectResult ParentEffectResult { get; }
    }

    public EffectQueue()
    {
        Initialize();
    }

    private void Initialize()
    {
        InitializeQueue();
        InitializeHistory();
    }

    public override void Process(double delta)
    {
        CheckAndProcess();
        base.Process(delta);
    }
    public void OnTurnBegin()
    {
        EnqueueEffect(new EffectInput(_turnBeginEffect, null, null));
    }

    public void OnTurnEnd()
    {
        EnqueueEffect(new EffectInput(_turnEndEffect, null, null));
    }

    public void OnRoundBegin()
    {
        EnqueueEffect(new EffectInput(_roundBeginEffect, null, null));
    }

    public void OnRoundEnd()
    {
        EnqueueEffect(new EffectInput(_roundEndEffect, null, null));
    }

    public void OnUnitDeath(IReadOnlyUnit killer, IReadOnlyUnit deadUnit)
    {
        EnqueueEffect(new EffectInput(_unitDeathEffect, killer, deadUnit), null, null, UnitSprite.DEATHANIMATIONDURATION);
    }

    private void InitializeQueue() 
    {
        _queue = new Queue<QueuedEffect>();
        _priorityQueue = new Queue<QueuedEffect>();
    }

    private void InitializeHistory()
    {
        _effectHistory = new DropOutStack<(EffectRequest, EffectResult)>(50);
        _moveHistory = new DropOutStack<MoveInput>(24);
    }

    private void CheckAndProcess()
    {
        if (_processingEnabled)
        {
            if (NextEffectReady(_priorityQueue))
            {
                DequeueAndProcess(_priorityQueue);
            }
            else if (NextEffectReady(_queue))
            {
                DequeueAndProcess(_queue);
            }
        }
    }

    private void DequeueAndProcess(Queue<QueuedEffect> queue)
    {
        QueuedEffect queuedEffect = queue.Dequeue();
        EffectInput effectInput = queuedEffect.EffectInput;

        RemoveChild(queuedEffect.VisualizationListener.Visualization);

        if (effectInput.Effect == null)
        {
            Trace.TraceError("Effect input missing effect");
            return;
        }

        if ((effectInput.Sender == null)
            && (effectInput.Effect.TargetingStyle != EffectTargetingStyle.PredeterminedGlobalTrigger))
        {
            Trace.TraceError("Effect input missing sender");
            return;
        }

        if ((effectInput.Target == null)
            && (effectInput.Effect.TargetingStyle != EffectTargetingStyle.PredeterminedGlobalTrigger))
        {
            Trace.TraceError("Effect input missing target");
            return;
        }

        if ((effectInput.Effect.TargetingStyle != EffectTargetingStyle.PredeterminedGlobalTrigger)
            && (!effectInput.Sender.InCombat))
        {
            Trace.TraceError("Effect sender no longer in combat");
            return;
        }

        if ((effectInput.Effect.TargetingStyle != EffectTargetingStyle.PredeterminedGlobalTrigger)
            && (!effectInput.Target.InCombat))
        {
            Trace.TraceError("Effect target no longer in combat");
            return;
        }

        ProcessEffect(effectInput);
    }

    public void EnqueueMove(MoveInput moveInput)
    {
        ReadOnlyCollection<Effect> effects = moveInput.Move.Effects;
        IReadOnlyUnit sender = moveInput.Sender;
        _lastMoveTarget = moveInput.Target;

        EnqueueEffect(new EffectInput(_moveBeginEffect, sender, _lastMoveTarget));
        foreach (Effect effect in effects)
        {
            EnqueueEffect(new EffectInput(effect, sender, _lastMoveTarget), null, null, UNIVERSALMOVEEFFECTDELAY);
        }
        EnqueueEffect(new EffectInput(_moveEndEffect, sender, _lastMoveTarget));

        _moveHistory.Push(moveInput);
    }

    public void EnqueueEffect(
        EffectInput effectInput,
        EffectRequest parentEffectRequest = null,
        EffectResult parentEffectResult = null,
        double additionalDelay = 0)
    {
        if (effectInput.Effect == null)
        {
            Trace.TraceError("Effect input missing effect");
            return;
        }

        if ((effectInput.Sender == null) 
            && (effectInput.Effect.TargetingStyle != EffectTargetingStyle.PredeterminedGlobalTrigger))
        {
            Trace.TraceError("Effect input missing sender");
            return;
        }

        if (effectInput.Effect.TargetingStyle != EffectTargetingStyle.PredeterminedGlobalTrigger) 
            effectInput.Target = DetermineTarget(effectInput, parentEffectRequest);

        if ((effectInput.Target == null) 
            && (effectInput.Effect.TargetingStyle != EffectTargetingStyle.PredeterminedGlobalTrigger))
        {
            Trace.TraceError("Could not determine effect target");
            return;
        }

        if (effectInput.Effect.SwapSenderAndTarget)
        {
            effectInput.SwapSenderAndTarget();
        }

        QueuedEffect queuedEffect = new QueuedEffect(effectInput, parentEffectResult, additionalDelay);
        EffectVisualization visualization = queuedEffect.VisualizationListener.Visualization;
        
        if (visualization != null)
        {
            AddChild(visualization);
        }
        
        if (effectInput.Effect.RequiresPriority)
        {
            _priorityQueue.Enqueue(queuedEffect);
        }
        else
        {
            _queue.Enqueue(queuedEffect);
        }
    }

    private bool NextEffectReady(Queue<QueuedEffect> queue)
    {
        if (queue.Count == 0)
        {
            return false;
        }

        QueuedEffect nextQueuedEffect = queue.Peek();
        return nextQueuedEffect.VisualizationListener.ReadyForExecute;
    }

    private IReadOnlyUnit DetermineTarget(EffectInput effectInput, EffectRequest parentEffectResult)
    {
        switch (effectInput.Effect.TargetingStyle) 
        {
            case EffectTargetingStyle.MoveTarget:
                return _lastMoveTarget;
            case EffectTargetingStyle.Self:
                return effectInput.Sender;
            case EffectTargetingStyle.ParentTarget:
                return parentEffectResult.Target;
            case EffectTargetingStyle.ParentSender:
                return parentEffectResult.Sender;
            case EffectTargetingStyle.PredeterminedGlobalTrigger:
                return effectInput.Target;
            default:
                return null;
        }
    }

    public bool IsEmpty()
    {
        int totalCount = _queue.Count + _priorityQueue.Count;
        return (totalCount == 0);
    }

    //
    // Effect processing
    //
    public event EventHandler<ExecuteGlobalTriggerEffectEventArgs> ExecuteGlobalTriggerEffectEventHandler;

    public void ProcessEffect(EffectInput effectInput, EffectResult parentEffectResult = null)
    {
        GenerateEffectRequestArgs args = new GenerateEffectRequestArgs();
        args.ParentEffectResult = parentEffectResult;
        args.Target = effectInput.Target;
        args.Sender = effectInput.Sender;
        args.ChildEffects = effectInput.Effect.ChildEffects;
        args.SpecialArgs = effectInput.SpecialArgs;

        if (effectInput.Effect.TargetingStyle == EffectTargetingStyle.PredeterminedGlobalTrigger)
        {
            EffectRequest request = effectInput.Effect.GenerateEffectRequest(args);

            ExecuteGlobalTriggerEffectEventArgs e = new();
            e.EffectRequest = request;

            ExecuteGlobalTriggerEffectEventHandler?.Invoke(this, e);
        }
        else
        {
            Unit senderAsUnit = effectInput.Sender as Unit;

            if (senderAsUnit == null)
            {
                Trace.TraceError("EffectQueue: Sender could not be cast as Unit");
            }

            senderAsUnit.Stats.SendEffect(effectInput.Effect.GenerateEffectRequest(args));
        }
    }

    public void ResolveEffect(EffectRequest request, EffectResult result)
    {
        if (request.ChildEffects != null)
        {
            foreach (Effect childEffect in request.ChildEffects)
            {
                EffectInput childEffectInput = new EffectInput(childEffect, request.Sender);
                EnqueueEffect(childEffectInput, request, result);
            }
        }

        _effectHistory.Push((request, result));
    }
}

public class ExecuteGlobalTriggerEffectEventArgs : EventArgs
{
    public EffectRequest EffectRequest { get; set; }
}
