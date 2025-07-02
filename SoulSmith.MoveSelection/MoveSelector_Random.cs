using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmith.Battle.Move;
using SoulSmith.Combat;
using SoulSmith.Core;
using SoulSmith.Units;
using SoulSmith.Battle;

namespace SoulSmith.MoveSelection;
public partial class MoveSelector_Random : MoveSelector
{
	public override void SelectMoveInput(IReadOnlyCombatTeam thisTeam, IReadOnlyCombatTeam enemyTeam)
	{
		base.SelectMoveInput(thisTeam, enemyTeam);
		SelectSender();
	}
	
	public override void SelectSender()
	{
		ReadOnlyCollection<IReadOnlyUnit> activeUnits = Team.GetActiveUnitsAsReadOnly();
		if (activeUnits.Count == 0)
		{
			PassTurn();
			return;
		}
		int selectedIndex = Rand.RandInt(activeUnits.Count);
		ReceiveSender(activeUnits[selectedIndex]);
        SelectMove();
    }
	
	public override void SelectMove()
	{
		IReadOnlyUnit userUnit = GetSender();
        ReadOnlyCollection<Move> moveSet = userUnit.MoveSet;
        if (moveSet.Count == 0)
        {
            PassTurn();
			return;
        }
        int selectedIndex = Rand.RandInt(moveSet.Count);
		ReceiveMove(moveSet[selectedIndex]);
        SelectTarget();
    }
	
	public override void SelectTarget()
	{
		List<IReadOnlyUnit> viableTargets = GetViableTargets();
        if (viableTargets.Count == 0)
        {
            PassTurn();
			return;
        }
        int selectedIndex = Rand.RandInt(viableTargets.Count);
		ReceiveTarget(viableTargets[selectedIndex]);
        ReturnMoveInputToCombatManager();
    }
}
