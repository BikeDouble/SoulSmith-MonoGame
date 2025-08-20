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
        public ReadOnlyCollection<IReadOnlyUnit> GetActiveUnitsAsReadOnly();
        public ReadOnlyCollection<IReadOnlyUnit> GetAdjacentReadOnlyUnits(IReadOnlyUnit target);
        public bool HasActiveUnit();
    }
}
