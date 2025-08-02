using System;
using SoulSmith.Battle.Moves;
using SoulSmith.Combat;
using SoulSmith.Battle;

namespace SoulSmith.MoveSelection;
public class MoveSelector_PlayerInput : MoveSelector
{
	
	//
	// Selecters
	//
	
	public override void SelectMoveInput(IReadOnlyCombatTeam thisTeam, IReadOnlyCombatTeam enemyTeam)
	{
		base.SelectMoveInput(thisTeam, enemyTeam);
		if (thisTeam.HasActiveUnit())
		{
			SelectMove();
		}
		else
		{
			PassTurn();
		}
	}
	
	public override void SelectMove()
	{
		ShowMoveSelectUI();
		ShowDeployUnitUI();
	}
	
	// Assumes move and user are selected already
	public override void SelectTarget()
	{
		MoveTargetingStyle targetingStyle = GetMove().TargetingStyle;
		ShowTargetSelectUI(targetingStyle, GetSender());
	}
	
	//
	// Receivers
	//
	
	public override void ReceiveMove(Move move)
	{
		SetMove(move);
		SelectTarget();
	}
	
	public override void ReceiveTarget(IReadOnlyUnit target)
	{
		SetTarget(target);
		ReturnMoveInputToCombatManager();
	}

    public override bool PlayerControlled { get { return true; } }
}
