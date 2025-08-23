using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SoulSmith.Battle.Effects;

namespace SoulSmith.Battle
{
    public interface IReadOnlyCombat : IEffectOriginator
    {
        public IReadOnlyCombatTeam GetReadOnlyTeamWithUnit(IReadOnlyUnit unit);
        public IReadOnlyCombatTeam GetEnemyReadOnlyTeam(IReadOnlyCombatTeam team);
        public IReadOnlyCombatTeam GetEnemyReadOnlyTeam(IReadOnlyUnit unit);
        public ReadOnlyCollection<IReadOnlyUnit> GetAllActiveUnitsAsReadOnly();
        public ReadOnlyCollection<IReadOnlyUnit> GetAllReadOnlyUnits();
        public IReadOnlyUnit GetReadOnlyUnitAcrossFrom(IReadOnlyUnit unit);
        public IReadOnlyUnit GetAnyEnemyReadOnlyUnit(IReadOnlyUnit unit);
    }
}
