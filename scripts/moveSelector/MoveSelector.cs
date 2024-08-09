using System;
using System.Collections.Generic;
using System.Diagnostics;
using SoulSmithMoves;

public partial class MoveSelector 
{	
	private MoveInput _moveInput;
	private bool _turnPassed;
	private Team _thisTeam;
	private Team _enemyTeam;

	public event EventHandler<OfferCompleteMoveInputEventArgs> OfferCompleteMoveInputEventHandler;

    public event EventHandler<ShowMoveSelectUIEventArgs> ShowMoveSelectUIEventHandler;

    public event EventHandler<ShowTargetSelectUIEventArgs> ShowTargetSelectUIEventHandler;

	protected void ShowTargetSelectUI(MoveTargetingStyle targetingStyle, IReadOnlyUnit sender)
	{
		ShowTargetSelectUIEventArgs e = new ShowTargetSelectUIEventArgs();
		e.TargetingStyle = targetingStyle;
		e.Sender = sender;

		ShowTargetSelectUIEventHandler(this, e);
	}

	protected void ShowMoveSelectUI()
	{
		ShowMoveSelectUIEventArgs e = new ShowMoveSelectUIEventArgs();
        ShowMoveSelectUIEventHandler(this, e);
    }

    protected void ReturnMoveInputToCombatManager()
	{
		if (_turnPassed)
		{
			return;
		}

		if ((_moveInput.Sender != null) && (_moveInput.Target != null))
		{
			OfferCompleteMoveInputEventArgs e = new OfferCompleteMoveInputEventArgs();
			e.MoveInput = _moveInput;

			OfferCompleteMoveInputEventHandler(this, e);
		}
	}

    public event EventHandler<OfferPassTurnEventArgs> OfferPassTurnEventHandler;

    protected void PassTurn()
	{
		if (_turnPassed) 
		{ 
			return; 
		}

		_turnPassed = true;
		OfferPassTurnEventArgs e = new OfferPassTurnEventArgs();
		OfferPassTurnEventHandler(this, e);
	}
	
	public virtual void OnTeamJoinCombat()
	{
		
	}
	
	//
	// Selecters
	//
	
	public virtual void SelectMoveInput(Team thisTeam, Team enemyTeam)
	{
		_thisTeam = thisTeam;
		_enemyTeam = enemyTeam;
		_turnPassed = false;
	}
	
	public virtual void SelectMove()
	{
		Trace.TraceError("Bad SelectMove call in MoveSelectionLogic");
	}
	
	public virtual void SelectSender()
	{
        Trace.TraceError("Bad SelectUser call in MoveSelectionLogic");
	}
	
	public virtual void SelectTarget()
	{
        Trace.TraceError("Bad SelectUser call in MoveSelectionLogic");
	}
	
	//
	// Receivers
	//
	
	public virtual void ReceiveMove(Move move)
	{
		SetMove(move);
	}
	
	public virtual void ReceiveSender(IReadOnlyUnit user)
	{
		SetUser(user);
	}
	
	public virtual void ReceiveTarget(IReadOnlyUnit target)
	{
		SetTarget(target);
	}
	
	//
	// Setters
	//
	
	public void SetMove(Move move)
	{
		_moveInput.Move = move;
	}
	
	public void SetUser(IReadOnlyUnit user)
	{
		_moveInput.Sender = user;
	}
	
	public void SetTarget(IReadOnlyUnit target)
	{
		_moveInput.Target = target;
	}
	
	//
	// Getters
	//
	
	public Move GetMove()
	{
		return _moveInput.Move;
	}
	
	public IReadOnlyUnit GetUser()
	{
		return _moveInput.Sender;
	}
	
	public IReadOnlyUnit GetTarget()
	{
		return _moveInput.Target;
	}
	
	/*public Array<TeamPosition> GetPositions()
	{
		Team thisTeam = Team;
		return thisTeam.GetPositions();
	}
	
	public Array<TeamPosition> GetEnemyPositions()
	{
		Team thisTeam = Team;
		CombatManager combatManager = ((CombatManager)thisTeam.GetParent());
		Team enemyTeam = combatManager.GetEnemyTeam(thisTeam);
		Array<TeamPosition> enemyPositions = enemyTeam.GetPositions();
		
		return enemyPositions;
	}*/
	
	/// <summary>
	/// Returns list of all viable targets, assuming move and user are already selected.
	/// </summary>
	/// <returns></returns>
	public List<IReadOnlyUnit> GetViableTargets()
	{
		MoveTargetingStyle targetingStyle = GetMove().TargetingStyle;
		
		List<IReadOnlyUnit> viableTargets;
		
		switch (targetingStyle)
		{
			case MoveTargetingStyle.AllyOrSelf:
				viableTargets = Team.GetReadOnlyUnits();
				return viableTargets;
			case MoveTargetingStyle.Ally:
				viableTargets = Team.GetReadOnlyUnits();
				viableTargets.Remove(GetUser());
				return viableTargets;
			case MoveTargetingStyle.Enemy:
				viableTargets = EnemyTeam.GetReadOnlyUnits();
				return viableTargets;
			default:
				viableTargets = new List<IReadOnlyUnit>();
				return viableTargets;
		}
	}

	public IReadOnlyUnit User { get { return _moveInput.Sender; } }
	public Team Team { get { return _thisTeam; } }
	public Team EnemyTeam { get { return _enemyTeam; } }
}

public class ShowTargetSelectUIEventArgs : EventArgs
{
    public MoveTargetingStyle TargetingStyle;
    public IReadOnlyUnit Sender;
}

public class OfferCompleteMoveInputEventArgs : EventArgs
{
	public MoveInput MoveInput;
}

public class ShowMoveSelectUIEventArgs : EventArgs
{

}

public class OfferPassTurnEventArgs : EventArgs
{

}