using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Core
{
    public enum ZLayer
    {
        Background = -1000,
        UnitSprite = 0,
        UnitUI = 1000,
        EffectVisualization = 2000,
        Header = 10000
    }
}
