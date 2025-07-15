using SoulSmith.Battle.Effect.Visualization;
using SoulSmith.Battle.Modifier;
using SoulSmith.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effect.Modifier
{
    public class VisualizedModifierEffectBase : VisualizedEffectBase
    {
        public VisualizedModifierEffectBase(ModifierAlignment modifierAlignment, bool isModifierVisible, DrawableResourceKey modifierIconKey, EffectVisualization visualization) : base(visualization)
        {
            ModifierAlignment = modifierAlignment;
            IsModifierVisible = isModifierVisible;
            ModifierIconKey = modifierIconKey;
        }

        public ModifierAlignment ModifierAlignment { get; private set; }
        public bool IsModifierVisible { get; private set; }
        public DrawableResourceKey ModifierIconKey { get; private set; }
    }
}
