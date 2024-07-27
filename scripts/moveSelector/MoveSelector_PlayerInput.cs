using System;
using SoulSmithMoves;

public partial class MoveSelector_PlayerInput : MoveSelector
{
	
	//
	// Selecters
	//
	
	public override void SelectMoveInput(Team thisTeam, Team enemyTeam)
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
	
	public override void ReceiveTarget(Unit target)
	{
		SetTarget(target);
		ReturnMoveInputToCombatManager();
	}
}
