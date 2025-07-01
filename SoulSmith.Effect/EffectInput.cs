using System;
using SoulSmith.Battle;
using System.Collections.ObjectModel;

namespace SoulSmith.Effect
{
    public struct EffectInput
    {
        public EffectInput(IEffect effect, IReadOnlyUnit sender, IReadOnlyUnit target = null, IList<float> specialArgs = null)
        {
            Effect = effect;
            Sender = sender;
            Target = target;

            if (specialArgs != null)
            {
                SpecialArgs = new ReadOnlyCollection<float>(specialArgs);
            }
        }

        public void SwapSenderAndTarget()
        {
            IReadOnlyUnit temp = Sender;
            Sender = Target;
            Target = temp;
        }

        public IEffect Effect;
        public IReadOnlyUnit Sender;
        public IReadOnlyUnit Target;
        public ReadOnlyCollection<float> SpecialArgs = null;
    }

    public class EnqueueEffectInputEventArgs : EventArgs
    {
        public EffectInput EffectInput;
    }
}
