using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Object.Canvas;
using SoulSmith.Battle;

namespace SoulSmith.Game;

public class UnitListUIEntry : CanvasObject
{
    private string _unitName;
    private IReadOnlyUnit _unit;

    public UnitListUIEntry(
        IReadOnlyUnit unit)
    {
        _unit = unit;
        

    }

    private string test;

    public IReadOnlyUnit Unit { get { return _unit; } }
}

