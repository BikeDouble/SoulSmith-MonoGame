using System;
using SoulSmith.Battle.Move;
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
	}
	
	// Assumes move and user are selected already
	public override void SelectTarget()
	{
		MoveTargetingStyle targetingStyle = GetMove().TargetingStyle;
		ShowTargetSelectUI(targetingStyle, GetUser());
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
}
