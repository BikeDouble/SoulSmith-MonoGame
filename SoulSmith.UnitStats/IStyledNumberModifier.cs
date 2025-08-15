using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.UnitStats
{
    public interface IStyledNumberModifier
    {
        float ModAmount { get; }
        StatModStyle ModStyle { get; }
    }
}
