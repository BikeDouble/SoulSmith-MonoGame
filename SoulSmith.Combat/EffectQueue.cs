using SoulSmith.Battle.Moves;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Visualization;
using SoulSmith.Object.Canvas;
using System.Diagnostics;
using System.Collections.ObjectModel;
using SoulSmith.Collections;
using SoulSmith.Units;
using SoulSmith.Battle;
using SoulSmith.Object;
using SoulSmith.Battle.Effects.Trigger;

namespace SoulSmith.Combat;
public class EffectQueue : CanvasObject
{
    public const double UNIVERSALMOVEEFFECTDELAY = UnitSprite.ATTACKANIMATIONDURATION / 2;
    public static readonly ReadOnlyCollection<Priority> PRIORITYORDER = new ReadOnlyCollection<Priority>(
        new List<Priority>
        {
            Priority.EmotionCombatEntryEffect,
            Priority.ImmediateAfterEffect,
            Priority.Reaction,
            Priority.Common,
            Priority.ModifierRemoval,
        });

    private Dictionary<Priority, Queue<QueuedEffect>> _priorityQueues = new Dictionary<Priority, Queue<QueuedEffect>>();
    private DropOutStack<(EffectRequest, EffectResult)> _effectHistory; //Effect history is pushed after effect is processed
    private DropOutStack<MoveInput> _moveHistory; //Move history is pushed after move is queued
    private bool _processingEnabled = true;
    private IReadOnlyCombat _parentCombat;
    private GlobalTriggerEffect _moveBeginEffect = new GlobalTriggerEffect(CombatTrigger.OnMoveBegin);
    private GlobalTriggerEffect _moveEndEffect = new GlobalTriggerEffect(CombatTrigger.OnMoveEnd);
    private GlobalTriggerEffect _roundBeginEffect = new GlobalTriggerEffect(CombatTrigger.OnRoundBegin);
    private GlobalTriggerEffect _roundEndEffect = new GlobalTriggerEffect(CombatTrigger.OnRoundEnd);
    private GlobalTriggerEffect _turnBeginEffect = new GlobalTriggerEffect(CombatTrigger.OnTurnBegin);
    private GlobalTriggerEffect _turnEndEffect = new GlobalTriggerEffect(CombatTrigger.OnTurnEnd);
    private GlobalTriggerEffect _unitDeathEffect = new GlobalTriggerEffect(CombatTrigger.OnUnitDeath);

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
        InitializeQueues();
        InitializeHistory();
    }

    public override void Process(double delta)
    {
        bool effectProcessed = CheckAndProcess();
        while (effectProcessed)
        {
            effectProcessed = CheckAndProcess();
        }
        base.Process(delta);
        RemoveVisualizationsInList();
    }

    public void OnTurnBegin()
    {
        EnqueueEffect(new EffectInput(_turnBeginEffect, null, null, Priority.Common));
    }

    public void OnTurnEnd()
    {
        EnqueueEffect(new EffectInput(_turnEndEffect, null, null, Priority.Common));
    }

    public void OnRoundBegin()
    {
        EnqueueEffect(new EffectInput(_roundBeginEffect, null, null, Priority.Common));
    }

    public void OnRoundEnd()
    {
        EnqueueEffect(new EffectInput(_roundEndEffect, null, null, Priority.Common));
    }

    public void OnUnitDeath(IReadOnlyUnit killer, IReadOnlyUnit deadUnit, EffectResult killingEffectResult)
    {
        EnqueueEffect(new EffectInput(_unitDeathEffect, killer, deadUnit, Priority.Common), killingEffectResult, UnitSprite.DEATHANIMATIONDURATION);
    }

    private void InitializeQueues() 
    {
        foreach (Priority priority in PRIORITYORDER)
        {
            _priorityQueues[priority] = new Queue<QueuedEffect>();
        }
    }

    private void InitializeHistory()
    {
        _effectHistory = new DropOutStack<(EffectRequest, EffectResult)>(50);
        _moveHistory = new DropOutStack<MoveInput>(24);
    }

    private bool CheckAndProcess()
    {
        if (_processingEnabled)
        {
            foreach (Priority priority in PRIORITYORDER)
            {
                Queue<QueuedEffect> queue = _priorityQueues[priority];
                if (NextEffectReady(queue))
                {
                    DequeueAndProcess(queue);
                    return true;
                }
            }
        }

        return false;
    }

    private void DequeueAndProcess(Queue<QueuedEffect> queue)
    {
        QueuedEffect queuedEffect = queue.Dequeue();
        EffectInput effectInput = queuedEffect.EffectInput;

        if (effectInput.Effect == null)
        {
            Trace.TraceError("Effects input missing effect");
            return;
        }

        SendEffectRequestFromInput(effectInput);
    }

    public void EnqueueMove(MoveInput moveInput)
    {
        ReadOnlyCollection<IEffect> effects = moveInput.Move.Effects;
        IReadOnlyUnit sender = moveInput.Sender;
        IReadOnlyUnit target = moveInput.Target;

        EnqueueEffect(new EffectInput(_moveBeginEffect, sender, target, Priority.Common));
        foreach (IEffect effect in effects)
        {
            EnqueueEffect(new EffectInput(effect, sender, target, Priority.Common), null, UNIVERSALMOVEEFFECTDELAY);
        }
        EnqueueEffect(new EffectInput(_moveEndEffect, sender, target, Priority.Common));

        _moveHistory.Push(moveInput);
    }

    public void EnqueueEffect(
        EffectInput effectInput,
        EffectResult parentEffectResult = null,
        double additionalDelay = 0)
    {
        if (effectInput.Effect == null)
        {
            throw new ArgumentNullException(nameof(effectInput.Effect), "EffectInput must have a valid effect.");
        }

        QueuedEffect queuedEffect = new QueuedEffect(effectInput, parentEffectResult, additionalDelay);
        EffectVisualization visualization = queuedEffect.VisualizationListener.Visualization;
        
        if (visualization != null)
        {
            AddChild(visualization);
        }
        
        Queue<QueuedEffect> queue = _priorityQueues[effectInput.EnqueuePriority];

        if (queue == null)
        {
            throw new ArgumentException($"No queue found for priority {effectInput.EnqueuePriority}");
        }

        queue.Enqueue(queuedEffect);
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
        foreach (Priority priority in PRIORITYORDER)
        {
            int count = _priorityQueues[priority].Count;
            if (count > 0)
            {
                return false;
            }
        }
        return true;
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
                EffectInput immediateAfterEffectInput = new EffectInput(immediateAfterEffect, request.Sender, request.Target, Priority.ImmediateAfterEffect);
                EnqueueEffect(immediateAfterEffectInput, result);
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
