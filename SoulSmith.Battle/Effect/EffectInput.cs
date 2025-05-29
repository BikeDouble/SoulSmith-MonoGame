using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effect
{
    public struct EffectInput
    {
        public EffectInput(Effect effect, IReadOnlyUnit sender, IReadOnlyUnit target = null, IList<float> specialArgs = null)
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

        public Effect Effect;
        public IReadOnlyUnit Sender;
        public IReadOnlyUnit Target;
        public ReadOnlyCollection<float> SpecialArgs = null;
    }

    public class EnqueueEffectInputEventArgs : EventArgs
    {
        public EffectInput EffectInput;
    }
}
