using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Battle
{
    public interface IReadOnlyCombatTeam
    {
        public List<IReadOnlyUnit> GetReadOnlyUnits();
        public List<IReadOnlyUnit> GetActiveUnitsAsReadOnly();
        public List<IReadOnlyUnit> GetAdjacentReadOnlyUnits(IReadOnlyUnit target);
        public bool HasActiveUnit();
    }
}
