using SoulSmith.Battle;
using SoulSmith.Core;
using SoulSmith.Object.Canvas;
using SoulSmith.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Camps
{
    public class CampBuildingMenuSlot : ButtonObject
    {
        // Children
        private Unit _unit;

        public CampBuildingMenuSlot(string idleResourceKey, string hoveredResourceKey, Position position = null) : base(idleResourceKey, hoveredResourceKey, position)
        {
        }

        public void GiveUnit(Unit unit)
        {
            this._unit = unit;
            AddChild(unit);
        }

        public void ClearUnit()
        {
            RemoveChild(this._unit);
            this._unit = null;
        }

        public Unit Unit { get { return _unit; } }
        public bool HasUnit { get { return Unit != null; } }
    }
}
