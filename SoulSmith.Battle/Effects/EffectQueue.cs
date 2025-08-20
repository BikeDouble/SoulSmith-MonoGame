using SoulSmith.Battle.Moves;
using SoulSmith.Battle.Effects.Visualization;
using SoulSmith.Object.Canvas;
using System.Diagnostics;
using System.Collections.ObjectModel;
using SoulSmith.Collections;
using SoulSmith.Object;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Core;
using SoulSmith.Battle.Effects.Results;
using SoulSmith.Battle.Effects.Payloads;

namespace SoulSmith.Battle.Effects;
public class EffectQueue : CanvasObject
{
    public const int EFFECTVISUALIZATIONZVALUE = (int)ZLayer.EffectVisualization;
    public const double ATTACKANIMATIONDURATION = 2;
    public const double DEATHANIMATIONDURATION = 2;
    public const double UNIVERSALMOVEEFFECTDELAY = ATTACKANIMATIONDURATION / 2;
    public static readonly ReadOnlyCollection<Priority> PRIORITYORDER = new ReadOnlyCollection<Priority>(
        new List<Priority>
        {
            Priority.NonMoveCombatTrigger,
            Priority.Body,
            Priority.EmotionCombatEntryEffect,
            Priority.ModifierRemovalImmediate,
            Priority.ImmediateAfterEffect,
            Priority.Reaction,
            Priority.Move,
            Priority.ModifierRemovalDelayed,
        });

    private Dictionary<Priority, Queue<QueuedEffect>> _priorityQueues = new Dictionary<Priority, Queue<QueuedEffect>>();
    private DropOutStack<(PayloadBase, ResultBase)> _effectHistory; //Effect history is pushed after effect is processed
    private DropOutStack<MoveInput> _moveHistory; //Move history is pushed after move is queued
    private bool _processingEnabled = true;
    private IReadOnlyCombat _parentCombat;
    private GlobalTriggerEffect _moveBeginEffect = new GlobalTriggerEffect(CombatTrigger.OnMoveBegin);
    private GlobalTriggerEffect _moveEndEffect = new GlobalTriggerEffect(CombatTrigger.OnMoveEnd);
    private GlobalTriggerEffect _roundBeginEffect = new GlobalTriggerEffect(CombatTrigger.OnRoundBegin);
    private GlobalTriggerEffect _roundEndEffect = new GlobalTriggerEffect(CombatTrigger.OnRoundEnd);
    private GlobalTriggerEffect _turnBeginEffect = new GlobalTriggerEffect(CombatTrigger.OnTurnBegin);
    private GlobalTriggerEffect _turnEndEffect = new GlobalTriggerEffect(CombatTrigger.OnTurnEnd);
    public static readonly GlobalTriggerEffect UNITDEATHEFFECT = new GlobalTriggerEffect(CombatTrigger.OnUnitDeath);
    public static readonly GlobalTriggerEffect UNITRETREATEFFECT = new GlobalTriggerEffect(CombatTrigger.OnUnitRetreat);

    public readonly struct QueuedEffect
    {
        public QueuedEffect(EffectInput input, double additionalDelay)
        {
            EffectInput = input;
            VisualizationListener = new EffectVisualizationListener(input, additionalDelay);
        }

        public EffectInput EffectInput { get; }
        public EffectVisualizationListener VisualizationListener { get; }
    }

    public EffectQueue(IReadOnlyCombat parentCombat)
    {
        _parentCombat = parentCombat;
        Initialize();
        this.SetZ(EFFECTVISUALIZATIONZVALUE);
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
        EnqueueEffect(new EffectInput(_turnBeginEffect, null, null, Priority.NonMoveCombatTrigger, _parentCombat, null));
    }

    public void OnTurnEnd()
    {
        EnqueueEffect(new EffectInput(_turnEndEffect, null, null, Priority.NonMoveCombatTrigger, _parentCombat, null));
    }

    public void OnRoundBegin()
    {
        EnqueueEffect(new EffectInput(_roundBeginEffect, null, null, Priority.NonMoveCombatTrigger, _parentCombat, null));
    }

    public void OnRoundEnd()
    {
        EnqueueEffect(new EffectInput(_roundEndEffect, null, null, Priority.NonMoveCombatTrigger, _parentCombat, null));
    }

    public void OnUnitDeath(IReadOnlyUnit killer, IReadOnlyUnit deadUnit, ResultBase killingEffectResult)
    {
        EnqueueEffect(new EffectInput(UNITDEATHEFFECT, killer, deadUnit, Priority.NonMoveCombatTrigger, _parentCombat, killingEffectResult), DEATHANIMATIONDURATION);
    }

    public void OnUnitRetreat(IReadOnlyUnit retreatingUnit)
    {
        EnqueueEffect(new EffectInput(UNITRETREATEFFECT, null, retreatingUnit, Priority.NonMoveCombatTrigger, _parentCombat, null));
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
        _effectHistory = new DropOutStack<(PayloadBase, ResultBase)>(50);
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

        EnqueueEffect(new EffectInput(_moveBeginEffect, sender, target, Priority.Move, moveInput.Move, null));

        foreach (IEffect effect in effects)
        {
            EnqueueEffect(new EffectInput(effect, sender, target, Priority.Move, moveInput.Move, null), UNIVERSALMOVEEFFECTDELAY);
        }

        EnqueueEffect(new EffectInput(_moveEndEffect, sender, target, Priority.Move, moveInput.Move, null));

        _moveHistory.Push(moveInput);
    }

    public void EnqueueEffect(
        EffectInput effectInput,
        double additionalDelay = 0)
    {
        if (effectInput.Effect == null)
        {
            throw new ArgumentNullException(nameof(effectInput.Effect), "EffectInput must have a valid effect.");
        }

        QueuedEffect queuedEffect = new QueuedEffect(effectInput, additionalDelay);
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
    public event EventHandler<ExecuteEffectEventArgs> ExecuteEffectEventHandler;

    public void SendEffectRequestFromInput(EffectInput effectInput, ResultBase parentEffectResult = null)
    {
        PayloadBase payload = effectInput.Effect.GeneratePayload(effectInput.Sender, effectInput.Target, _parentCombat, effectInput.Originator, effectInput.ParentResult);

        if (payload == null) return;

        ExecuteEffectEventArgs e = new();
        e.Payload = payload;

        ExecuteEffectEventHandler?.Invoke(this, e);
    }

    /// <summary>
    /// Receives request and result of executed effect and enqueues its immediate after effects, with priority.
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="result"></param>
    public void ResolveEffect(PayloadBase payload, ResultBase result)
    {
        if (payload.ImmediateAfterEffects != null)
        {
            foreach (IEffect immediateAfterEffect in payload.ImmediateAfterEffects)
            {
                EffectInput immediateAfterEffectInput = new EffectInput(immediateAfterEffect, payload.Sender, payload.Target, Priority.ImmediateAfterEffect, result.Originator, result);
                EnqueueEffect(immediateAfterEffectInput);
            }
        }

        if (payload.Originator is IReadOnlyMove move) // Set result of next effect in move to this
        {
            if (_priorityQueues[Priority.Move].Count > 0)
            {
                EffectInput nextMoveInput = _priorityQueues[Priority.Move].Peek().EffectInput;
                if (nextMoveInput.Originator == move)
                {
                    nextMoveInput.ParentResult = result;
                }
            }
        }

        _effectHistory.Push((payload, result));
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

public class ExecuteEffectEventArgs : EventArgs
{
    public PayloadBase Payload { get; set; }
}
