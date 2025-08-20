using System;
using SoulSmith.Battle;
using System.Collections.ObjectModel;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Battle.Effects
{
    public class EffectInput
    {
        public EffectInput(IEffect effect, IReadOnlyUnit sender, IReadOnlyUnit target, Priority enqueuePriority, IEffectOriginator originator, ResultBase parentResult)
        {
            Effect = effect;
            Sender = sender;
            Target = target;
            EnqueuePriority = enqueuePriority;
            Originator = originator;
            ParentResult = parentResult;
        }

        public readonly IEffect Effect;
        public readonly IReadOnlyUnit Sender;
        public readonly IReadOnlyUnit Target;
        public readonly IEffectOriginator Originator;
        public readonly Priority EnqueuePriority;
        public ResultBase ParentResult;
    }

    public class EnqueueEffectInputEventArgs : EventArgs
    {
        public EffectInput EffectInput;
    }
}
