using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Battle.Effect;

namespace SoulSmith.Battle.Move
{
    public struct MoveInput
    {
        public Move Move;
        public IReadOnlyUnit Sender;
        public IReadOnlyUnit Target;
    }
}

