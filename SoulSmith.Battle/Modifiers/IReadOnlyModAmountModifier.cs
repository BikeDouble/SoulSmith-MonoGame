using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SoulSmith.Battle.Modifiers
{
    public interface IReadOnlyModAmountModifier : IReadOnlyModifier
    {
        public float ModAmount { get; }
    }
}
