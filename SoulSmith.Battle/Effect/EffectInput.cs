using System;
using SoulSmith.Battle;
using System.Collections.ObjectModel;

namespace SoulSmith.Battle.Effect
{
    public struct EffectInput
    {
        public EffectInput(IEffect effect, IReadOnlyUnit sender, IReadOnlyUnit target, bool enqueueWithPriority = false)
        {
            Effect = effect;
            Sender = sender;
            Target = target;
            EnqueueWithPriority = enqueueWithPriority;
        }

        public readonly IEffect Effect;
        public readonly IReadOnlyUnit Sender;
        public readonly IReadOnlyUnit Target;
        public readonly bool EnqueueWithPriority;
    }

    public class EnqueueEffectInputEventArgs : EventArgs
    {
        public EffectInput EffectInput;
    }
}
