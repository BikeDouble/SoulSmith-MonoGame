using SoulSmith.Battle.Move;
using SoulSmith.Battle.Effect;
using SoulSmith.Battle.Effect.Visualization;
using SoulSmith.Object.Canvas;
using System.Diagnostics;
using System.Collections.ObjectModel;
using SoulSmith.Collections;
using SoulSmith.Units;
using SoulSmith.Battle;
using SoulSmith.Object;

namespace SoulSmith.Combat;
public class EffectQueue : CanvasObject
{
    public const double UNIVERSALMOVEEFFECTDELAY = UnitSprite.ATTACKANIMATIONDURATION / 2;

    private Queue<QueuedEffect> _queue;
    private Queue<QueuedEffect> _priorityQueue;
    private DropOutStack<(EffectRequest, EffectResult)> _effectHistory; //Effect history is pushed after effect is processed
    private DropOutStack<MoveInput> _moveHistory; //Move history is pushed after move is queued
    private bool _processingEnabled = true;
    private IReadOnlyCombat _parentCombat;
    private GlobalTriggerEffect _moveBeginEffect = new GlobalTriggerEffect(EffectTrigger.OnMoveBegin);
    private GlobalTriggerEffect _moveEndEffect = new GlobalTriggerEffect(EffectTrigger.OnMoveEnd);
    private GlobalTriggerEffect _roundBeginEffect = new GlobalTriggerEffect(EffectTrigger.OnRoundBegin);
    private GlobalTriggerEffect _roundEndEffect = new GlobalTriggerEffect(EffectTrigger.OnRoundEnd);
    private GlobalTriggerEffect _turnBeginEffect = new GlobalTriggerEffect(EffectTrigger.OnTurnBegin);
    private GlobalTriggerEffect _turnEndEffect = new GlobalTriggerEffect(EffectTrigger.OnTurnEnd);
    private GlobalTriggerEffect _unitDeathEffect = new GlobalTriggerEffect(EffectTrigger.OnUnitDeath);

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

    public EffectQueue(IReadOnlyCombat parentCombat)
    {
        _parentCombat = parentCombat;
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
        RemoveVisualizationsInList();
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

        if (effectInput.Effect == null)
        {
            Trace.TraceError("Effect input missing effect");
            return;
        }

        SendEffectRequestFromInput(effectInput);
    }

    public void EnqueueMove(MoveInput moveInput)
    {
        ReadOnlyCollection<IEffect> effects = moveInput.Move.Effects;
        IReadOnlyUnit sender = moveInput.Sender;
        IReadOnlyUnit target = moveInput.Target;

        EnqueueEffect(new EffectInput(_moveBeginEffect, sender, target));
        foreach (IEffect effect in effects)
        {
            EnqueueEffect(new EffectInput(effect, sender, target), null, null, UNIVERSALMOVEEFFECTDELAY);
        }
        EnqueueEffect(new EffectInput(_moveEndEffect, sender, target));

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

        QueuedEffect queuedEffect = new QueuedEffect(effectInput, parentEffectResult, additionalDelay);
        EffectVisualization visualization = queuedEffect.VisualizationListener.Visualization;
        
        if (visualization != null)
        {
            AddChild(visualization);
        }
        
        if (effectInput.EnqueueWithPriority)
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

    public bool IsEmpty()
    {
        int totalCount = _queue.Count + _priorityQueue.Count;
        return (totalCount == 0);
    }

    //
    // Effect processing
    //
    public event EventHandler<SenderlessEffectEventArgs> ExecuteSenderlessEffectEventHandler;

    public void SendEffectRequestFromInput(EffectInput effectInput, EffectResult parentEffectResult = null)
    {
        if (effectInput.Sender == null)
        {
            EffectRequest request = effectInput.Effect.GenerateEffectRequest(effectInput.Sender, effectInput.Target, _parentCombat, parentEffectResult);

            SenderlessEffectEventArgs e = new();
            e.EffectRequest = request;

            ExecuteSenderlessEffectEventHandler?.Invoke(this, e);
        }
        else
        {
            Unit senderAsUnit = effectInput.Sender as Unit; //TODO make typesafe

            if (senderAsUnit == null)
            {
                Trace.TraceError("EffectQueue: Sender could not be cast as Unit");
            }

            senderAsUnit.Stats.SendEffect(effectInput.Effect.GenerateEffectRequest(effectInput.Sender, effectInput.Target, _parentCombat, parentEffectResult));
        }
    }

    /// <summary>
    /// Receives request and result of executed effect and enqueues its immediate after effects, with priority.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="result"></param>
    public void ResolveEffect(EffectRequest request, EffectResult result)
    {
        if (request.ImmediateAfterEffects != null)
        {
            foreach (IEffect immediateAfterEffect in request.ImmediateAfterEffects)
            {
                EffectInput immediateAfterEffectInput = new EffectInput(immediateAfterEffect, request.Sender, request.Target, true);
                EnqueueEffect(immediateAfterEffectInput, request, result);
            }
        }

        _effectHistory.Push((request, result));
    }

    private List<EffectVisualization> _visualizationsToRemove = new List<EffectVisualization>();

    private void RemoveVisualizationsInList()
    {
        foreach (EffectVisualization visualization in _visualizationsToRemove)
        {
            RemoveChild(visualization);
        }

        _visualizationsToRemove.Clear();
    }

    private void OnEndVisualization(object sender, EventArgs e)
    {
        EffectVisualization senderAsVisualization = sender as EffectVisualization;

        if (Children.Contains(senderAsVisualization))
        {
            _visualizationsToRemove.Add(senderAsVisualization);
        }
    }

    public override void AddChild(SoulSmithObject child)
    {
        if (child is EffectVisualization)
        {
            ((EffectVisualization)child).EndVisualizationEventHandler += OnEndVisualization;
        }

        base.AddChild(child);
    }

    public override void RemoveChild(SoulSmithObject child)
    {
        if (child is EffectVisualization)
        {
            ((EffectVisualization)child).EndVisualizationEventHandler -= OnEndVisualization;
        }

        base.RemoveChild(child);
    }
}

public class SenderlessEffectEventArgs : EventArgs
{
    public EffectRequest EffectRequest { get; set; }
}
