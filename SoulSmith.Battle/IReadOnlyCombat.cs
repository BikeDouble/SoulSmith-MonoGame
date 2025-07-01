using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SoulSmith.Battle
{
    public interface IReadOnlyCombat
    {
        public IReadOnlyCombatTeam GetReadOnlyTeamWithUnit(IReadOnlyUnit unit);
        public IReadOnlyCombatTeam GetEnemyReadOnlyTeam(IReadOnlyCombatTeam team);
        public IReadOnlyCombatTeam GetEnemyReadOnlyTeam(IReadOnlyUnit unit);
        public ReadOnlyCollection<IReadOnlyUnit> GetAllActiveUnitsAsReadOnly();
    }
}
