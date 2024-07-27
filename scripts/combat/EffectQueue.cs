using System.Collections.Generic;
using System;
using SoulSmithMoves;
using System.Diagnostics;
using System.Collections.ObjectModel;

public partial class EffectQueue : CanvasItem
{

    private Queue<QueuedEffect> _queue;
    private Queue<QueuedEffect> _priorityQueue;
    private DropOutStack<(EffectRequest, EffectResult)> _effectHistory; //Effect history is pushed after effect is processed
    private DropOutStack<MoveInput> _moveHistory; //Move history is pushed after move is queued
    private bool _processingEnabled = true;
    private Unit _lastMoveTarget;
    private Effect _moveBeginEffect = new Effect_Trigger(EffectTrigger.OnMoveBegin, EffectTargetingStyle.Self);
    private Effect _moveEndEffect = new Effect_Trigger(EffectTrigger.OnMoveEnd, EffectTargetingStyle.Self);
    private Effect _roundBeginEffect = new Effect_Trigger(EffectTrigger.OnRoundBegin, EffectTargetingStyle.Self);
    private Effect _roundEndEffect = new Effect_Trigger(EffectTrigger.OnRoundEnd, EffectTargetingStyle.Self);

    public readonly struct QueuedEffect
    {
        public QueuedEffect(EffectInput input)
        {
            EffectInput = input;
            VisualizationListener = new EffectVisualizationListener(input);
            ParentEffectResult = null;
        }

        public QueuedEffect(EffectInput input, EffectResult parentEffectResult)
        {
            EffectInput = input;
            VisualizationListener = new EffectVisualizationListener(input);
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

    public void OnRoundBegin(List<Unit> activeUnits)
    {
        EnqueueEffectForMultipleUnits(_roundBeginEffect, activeUnits);
    }

    public void OnRoundEnd(List<Unit> activeUnits)
    {
        EnqueueEffectForMultipleUnits(_roundEndEffect, activeUnits);
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

        if ((effectInput.Effect
             == null) || (effectInput.Sender == null) || (effectInput.Target == null))
        {
            Trace.TraceError("Effect input missing one or more parameters");
            return;
        }

        if (!effectInput.Sender.InCombat || !effectInput.Target.InCombat)
        {
            Trace.TraceError("Effect user or target no longer in combat");
            return;
        }

        ProcessEffect(effectInput);
    }

    public void EnqueueMove(MoveInput moveInput)
    {
        ReadOnlyCollection<Effect> effects = moveInput.Move.Effects;
        Unit user = moveInput.Sender;
        _lastMoveTarget = moveInput.Target;

        EnqueueEffect(new EffectInput(_moveBeginEffect, user, _lastMoveTarget));
        foreach (Effect effect in effects)
        {
            EnqueueEffect(new EffectInput(effect, user, _lastMoveTarget));
        }
        EnqueueEffect(new EffectInput(_moveEndEffect, user, _lastMoveTarget));

        _moveHistory.Push(moveInput);
    }

    public void EnqueueEffectForMultipleUnits(Effect effect, List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            EnqueueEffect(new EffectInput(effect, unit));
        }
    }

    public void EnqueueEffect(
        EffectInput effectInput,
        EffectRequest parentEffectRequest = null,
        EffectResult parentEffectResult = null)
    {
        if ((effectInput.Effect
             == null) || (effectInput.Sender == null))
        {
            Trace.TraceError("Effect input missing effect or user");
            return;
        }

        effectInput.Target = DetermineTarget(effectInput, parentEffectRequest);

        if (effectInput.Target == null)
        {
            Trace.TraceError("Could not determine effect target");
            return;
        }

        if (effectInput.Effect.SwapSenderAndTarget)
        {
            effectInput.SwapSenderAndTarget();
        }

        QueuedEffect queuedEffect = new QueuedEffect(effectInput, parentEffectResult);
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

    private Unit DetermineTarget(EffectInput effectInput, EffectRequest parentEffectResult)
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
            case EffectTargetingStyle.DeterminedPreOffer:
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
    public void ProcessEffect(EffectInput effectInput, EffectResult parentEffectResult = null)
    {
        Effect effect = effectInput.Effect;
        Unit user = effectInput.Sender;
        Unit target = effectInput.Target;

        user.Stats.SendEffect(effect.GenerateEffectRequest(user, target));
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
