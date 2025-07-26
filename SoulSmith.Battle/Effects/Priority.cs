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
        ModifierRemoval,
        SelfReaction,
        Reaction,
        ImmediateAfterEffect,
        EmotionCombatEntryEffect,
        NonMoveCombatTrigger,
        DecayDamage
    }
}
