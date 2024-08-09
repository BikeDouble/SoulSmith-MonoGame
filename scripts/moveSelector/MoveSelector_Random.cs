using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmithMoves;

public partial class MoveSelector_Random : MoveSelector
{
	public override void SelectMoveInput(Team thisTeam, Team enemyTeam)
	{
		base.SelectMoveInput(thisTeam, enemyTeam);
		SelectSender();
	}
	
	public override void SelectSender()
	{
		ReadOnlyCollection<Unit> activeUnits = Team.GetActiveUnits();
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
		IReadOnlyUnit userUnit = GetUser();
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
