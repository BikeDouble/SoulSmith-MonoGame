using System;
using SoulSmith.Battle;
using System.Collections.ObjectModel;

namespace SoulSmith.Battle.Effects
{
    public struct EffectInput
    {
        public EffectInput(IEffect effect, IReadOnlyUnit sender, IReadOnlyUnit target, Priority enqueuePriority)
        {
            Effect = effect;
            Sender = sender;
            Target = target;
            EnqueuePriority = enqueuePriority;
        }

        public readonly IEffect Effect;
        public readonly IReadOnlyUnit Sender;
        public readonly IReadOnlyUnit Target;
        public readonly Priority EnqueuePriority;
    }

    public class EnqueueEffectInputEventArgs : EventArgs
    {
        public EffectInput EffectInput;
        public EffectResult ParentEffectResult = null;
    }
}
