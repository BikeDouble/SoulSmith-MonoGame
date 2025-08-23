using System;
using System.Collections.Generic;
using System.Diagnostics;
using SoulSmith.Battle.Moves;
using SoulSmith.Combat;
using SoulSmith.Battle;

namespace SoulSmith.MoveSelection;
public class MoveSelector : IMoveSelector //TODO rework move selection
{	
	private MoveInput _moveInput;
	private bool _turnPassed;
	private IReadOnlyCombatTeam _thisTeam;
	private IReadOnlyCombatTeam _enemyTeam;

	public event EventHandler<OfferCompleteMoveInputEventArgs> OfferCompleteMoveInputEventHandler;

    public event EventHandler<ShowMoveSelectUIEventArgs> ShowMoveSelectUIEventHandler;

    public event EventHandler<ShowTargetSelectUIEventArgs> ShowTargetSelectUIEventHandler;

	public event EventHandler<ShowDeployUnitUIEventArgs> ShowDeployUnitUIEventHandler;

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

	protected void ShowDeployUnitUI()
	{
		ShowDeployUnitUIEventArgs e = new ShowDeployUnitUIEventArgs();
		ShowDeployUnitUIEventHandler(this, e);
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
	
	public virtual void SelectMoveInput(IReadOnlyCombatTeam thisTeam, IReadOnlyCombatTeam enemyTeam)
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
	
	public IReadOnlyUnit GetSender()
	{
		return _moveInput.Sender;
	}
	
	public IReadOnlyUnit GetTarget()
	{
		return _moveInput.Target;
	}
	
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
				viableTargets.Remove(GetSender());
				return viableTargets;
			case MoveTargetingStyle.Enemy:
				viableTargets = EnemyTeam.GetReadOnlyUnits();
				return viableTargets;
			case MoveTargetingStyle.Self:
				viableTargets = new List<IReadOnlyUnit> { GetSender() };
				return viableTargets;
            default:
				viableTargets = new List<IReadOnlyUnit>();
				return viableTargets;
		}
	}

	public IReadOnlyUnit User { get { return _moveInput.Sender; } }
	public IReadOnlyCombatTeam Team { get { return _thisTeam; } }
	public IReadOnlyCombatTeam EnemyTeam { get { return _enemyTeam; } }
	public virtual bool PlayerControlled { get { return false; } } 
}

