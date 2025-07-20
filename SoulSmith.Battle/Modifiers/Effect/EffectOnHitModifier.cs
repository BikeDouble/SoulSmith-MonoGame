using SoulSmith.Drawing;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Modifiers;
using SoulSmith.Battle.Effects.Visualization;

namespace SoulSmith.Battle.Modifiers.Effect
{
    public class EffectOnHitModifier : Modifier
    {
        private readonly IEffect _effect;

        public EffectOnHitModifier(
            IEffect effect,
            int duration,
            DurationStyle durationStyle,
            ModifierAlignment alignment,
            bool isVisible,
            DrawableResourceKey iconKey,
            string friendlyName,
            string description)
            : base(duration, durationStyle, alignment, isVisible, iconKey, friendlyName, description)
        {
            _effect = effect ?? throw new ArgumentNullException(nameof(effect));
        }

        public override void ReactToEffectResult(EffectResult result)
        {
            base.ReactToEffectResult(result);

            if (result == null) return;

            if (result.Sender != this.Host) return;

            if (result.Target == null) return;

            if (result.DamageType != Effects.Damage.DamageType.Hit) return;

            if (result.EffectiveDamage <= 0) return;

            EffectInput effectInput = new EffectInput(_effect, Host, result.Target, Priority.Reaction);

            EnqueueEffectInput(effectInput, result);
        }
    }
}
