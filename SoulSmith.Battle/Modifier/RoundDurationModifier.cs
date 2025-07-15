using SoulSmith.Battle.Effect;
using SoulSmith.Drawing;
using SoulSmith.Object.Canvas;

namespace SoulSmith.Battle.Modifier
{
    /// <summary>
    /// Base class for a Modifier that lasts for a specific number of rounds
    /// </summary>
    public class RoundDurationModifier : Modifier
    {
        public RoundDurationModifier(int duration, ModifierAlignment alignment = ModifierAlignment.Null, bool isVisible = true, DrawableResourceKey iconKey = null)
            : base(alignment, isVisible, iconKey)
        {
            Duration = duration;
        }
        public int Duration { get; private set; }
        public override void ProcessEffectResult(EffectResult result)
        {
            if (result.TriggerApplied == EffectTrigger.OnRoundEnd)
            {
                Duration -= 1;
                if (Duration <= 0)
                {
                    Remove();
                }
            }
        }
    }
}
