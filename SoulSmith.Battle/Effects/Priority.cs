using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects
{
    public enum Priority
    {
        Move,
        ModifierRemovalDelayed,
        ModifierRemovalImmediate,
        SelfReaction,
        Reaction,
        ImmediateAfterEffect,
        EmotionCombatEntryEffect,
        NonMoveCombatTrigger,
        NaturalDecayDamage
    }
}
