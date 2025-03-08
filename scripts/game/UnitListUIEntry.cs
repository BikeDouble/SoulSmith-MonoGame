using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SoulSmithUnitUI;

public class UnitListUIEntry : CanvasItem
{
    public UnitListUIEntry(
        IReadOnlyUnit unit)
    {
        _unit = unit;
    }

    private IReadOnlyUnit _unit;

    public IReadOnlyUnit Unit { get { return _unit; } }
}

