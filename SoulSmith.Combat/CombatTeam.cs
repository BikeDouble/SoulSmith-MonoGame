using System.Collections.ObjectModel;
using System.Diagnostics;
using SoulSmith.Object.Canvas;
using SoulSmith.Drawing;
using SoulSmith.Battle;
using SoulSmith.Battle.Moves;
using SoulSmith.Units;
using SoulSmith.Battle.Effects;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Combats;
public partial class CombatTeam : CanvasObject, IReadOnlyCombatTeam
{
	public const int BACKUNITSDISTANCEFROMSCREENEDGE = 250;
	public const int FRONTUNITSDISTANCEFROMSCREENEDGE = 400;
	public const int YOFFSET = 50;

    private IMoveSelector _moveSelector = null;
	private bool _playerControlled;
	private List<TeamPosition> _teamPositions = new List<TeamPosition>();

	public CombatTeam(IMoveSelector moveSelector, bool playerControlled) : base(new Core.Position(0, YOFFSET))
	{
		_moveSelector = moveSelector;

		_playerControlled = playerControlled;

		Initialize();
	}

	private void Initialize()
	{
		InitializeMoveSelector();
		InitializePositions();
	}

	private void InitializeMoveSelector()
	{
		_moveSelector.OfferCompleteMoveInputEventHandler += OnOfferCompleteMoveInput;
		_moveSelector.ShowMoveSelectUIEventHandler += OnShowMoveSelectUI;
		_moveSelector.ShowTargetSelectUIEventHandler += OnShowTargetSelectUI;
		_moveSelector.ShowDeployUnitUIEventHandler += OnShowDeployUnitUI;
		_moveSelector.OfferPassTurnEventHandler += OnOfferPassTurn;
	}

    private void InitializePositions()
	{
		if (_teamPositions.Count == 0)
		{
			if (_playerControlled)
			{
				_teamPositions.Add(new TeamPosition(BACKUNITSDISTANCEFROMSCREENEDGE, Window.WINDOWHEIGHT / 4, true));
				_teamPositions.Add(new TeamPosition(FRONTUNITSDISTANCEFROMSCREENEDGE, Window.WINDOWHEIGHT / 2, true));
				_teamPositions.Add(new TeamPosition(BACKUNITSDISTANCEFROMSCREENEDGE, 3 * Window.WINDOWHEIGHT / 4, true));
			}
			else
			{
				_teamPositions.Add(new TeamPosition(Window.WINDOWWIDTH - BACKUNITSDISTANCEFROMSCREENEDGE, Window.WINDOWHEIGHT / 4, false, -1, 1));
				_teamPositions.Add(new TeamPosition(Window.WINDOWWIDTH - FRONTUNITSDISTANCEFROMSCREENEDGE, Window.WINDOWHEIGHT / 2, false, -1, 1));
				_teamPositions.Add(new TeamPosition(Window.WINDOWWIDTH - BACKUNITSDISTANCEFROMSCREENEDGE, 3 * Window.WINDOWHEIGHT / 4, false, -1, 1));
			}
        }

		foreach (TeamPosition position in _teamPositions)
		{
			AddChild(position);
			position.OfferMoveAndUserEventHandler += OnOfferMoveAndUser;
			position.OfferTargetEventHandler += OnOfferTarget;
			position.DeployUnitButtonPressedEventHandler += OnDeployUnitButtonPressed;
			position.EnqueueEffectInputEventHandler += EnqueueEffectInput;
			position.UnitDeathCallEventHandler += OnUnitDeathCall;
			position.UnitRetreatCallEventHandler += OnUnitRetreatCall;
        }
	}

    public EventHandler<DeployUnitButtonPressedEventArgs> DeployUnitButtonPressedEventHandler;

    private void OnDeployUnitButtonPressed(object sender, DeployUnitButtonPressedEventArgs e)
    {
        DeployUnitButtonPressedEventHandler?.Invoke(this, e);
    }

    public event EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler;

    private void EnqueueEffectInput(object sender, EnqueueEffectInputEventArgs e)
    {
        EnqueueEffectInputEventHandler(this, e);
    }

	public void AssignUnitToPosition(Unit unit, int positionIndex)
	{
		if (unit == null)
		{
			throw new ArgumentNullException("Unit cannot be null.");
		}

		TeamPosition position = _teamPositions[positionIndex];

		AssignUnitToPosition(unit, position);
	}

    public void AssignUnitToPosition(Unit unit, IReadOnlyTeamPosition readOnlyPosition)
    {
        TeamPosition position = GetMatchingPosition(readOnlyPosition);

        AssignUnitToPosition(unit, position);
    }

    public void AssignUnitToPosition(Unit unit, TeamPosition position)
	{
        if (position.ContainsUnit)
        {
            throw new ArgumentException("Position already contains unit.");
        }

        position.AssignUnit(unit);
    }

	//
	// Listeners
	//

	public event EventHandler<UnitDeathCallArgs> UnitDeathCallEventHandler;

	private void OnUnitDeathCall(object sender, UnitDeathCallArgs e)
	{
		UnitDeathCallEventHandler(this, e);
	}

    public event EventHandler<UnitRetreatCallArgs> UnitRetreatCallEventHandler;

    private void OnUnitRetreatCall(object sender, UnitRetreatCallArgs e)
    {
        UnitRetreatCallEventHandler?.Invoke(this, e);
    }

    public void OnBeginRound()
	{
		foreach (TeamPosition position in _teamPositions)
		{
			position.OnBeginRound();
		}
	}

	public event EventHandler<OfferCompleteMoveInputEventArgs> OfferCompleteMoveInputEventHandler;

	//Listens to msl OfferMoveInput
	private void OnOfferCompleteMoveInput(object sender, OfferCompleteMoveInputEventArgs e)
	{
		OfferCompleteMoveInputEventHandler(this, e);
	}

	public event EventHandler<OfferPassTurnEventArgs> OfferPassTurnEventHandler;

	//Listens to msl OfferPassTurn
	private void OnOfferPassTurn(object sender, OfferPassTurnEventArgs e)
	{
		OfferPassTurnEventHandler(this, e);
	}

	public event EventHandler<ShowMoveSelectUIEventArgs> ShowMoveSelectUIEventHandler;

	//Listens to msl ShowMoveSelectUI
	private void OnShowMoveSelectUI(object sender, ShowMoveSelectUIEventArgs e)
	{
		ShowMoveSelectUIEventHandler(this, e);
	}

    public event EventHandler<ShowTargetSelectUIEventArgs> ShowTargetSelectUIEventHandler;

    //Listens to msl
    private void OnShowTargetSelectUI(object sender, ShowTargetSelectUIEventArgs e)
	{
		ShowTargetSelectUIEventHandler(this, e);
	}

	public event EventHandler<ShowDeployUnitUIEventArgs> ShowDeployUnitUIEventHandler;

	//Listens to msl
	private void OnShowDeployUnitUI(object sender, ShowDeployUnitUIEventArgs e)
	{
		ShowDeployUnitUIEventHandler?.Invoke(this, e);
	}

	public void ShowMoveSelectUI()
	{
		foreach (TeamPosition position in _teamPositions)
		{
			position.ShowMoveSelectUI();
		}
	}

	public void ShowTargetSelectUI(List<int> positionNumbers)
	{
		foreach (int positionNumber in positionNumbers)
		{
			_teamPositions[positionNumber].ShowTargetSelectUI();
		}
	}

	public void ShowDeployUnitUI()
	{
		foreach (TeamPosition position in _teamPositions)
		{
			if (!position.ContainsUnit) position.ShowDeployUnitUI();
		}
	}

    public event EventHandler<MoveButtonPressedEventArgs> OfferMoveAndUserEventHandler;

    //Connected to team position
    private void OnOfferMoveAndUser(object sender, MoveButtonPressedEventArgs args)
    {
        OfferMoveAndUserEventHandler(this, args);
    }

    public event EventHandler<TargetButtonPressedEventArgs> OfferTargetEventHandler;

    //Connected to team positions
    private void OnOfferTarget(object sender, TargetButtonPressedEventArgs args)
    {
		OfferTargetEventHandler(this, args);
    }


    public void AssignMoveAndUserToMSL(MoveButtonPressedEventArgs args)
	{
		_moveSelector.ReceiveSender(args.Sender);
		_moveSelector.ReceiveMove(args.Move);
	}

	public void GiveTargetToMSL(Unit target)
	{
		_moveSelector.ReceiveTarget(target);
	}

	public void PlayDeathAnimationForUnit(IReadOnlyUnit unit)
	{
		TeamPosition position = GetPositionWithUnit(unit);
		if (position != null)
		{
			position.PlayUnitDeathAnimation();
		}
    }

    public void HideMoveSelectUI()
	{
		foreach (TeamPosition position in _teamPositions)
		{
			position.HideMoveSelectUI();
		}
	}

	public void HideTargetSelectUI()
	{
		foreach (TeamPosition position in _teamPositions)
		{
			position.HideTargetSelectUI();
		}
	}

	//
	// Getters
	//
	
	/// <summary>
	/// Returns true if this team has at least one unit that can move this round
	/// </summary>
	/// <returns></returns>
	public bool HasActiveUnit()
	{
		foreach (TeamPosition position in _teamPositions)
		{
			if (!position.MovedThisRound && position.ContainsUnit)
			{
				return true;
			}
		}	
		
		return false;
	}

	/// <summary>
	/// Returns true iff this team contains specified unit
	/// </summary>
	/// <param name="unit"></param>
	/// <returns></returns>
	public bool ContainsUnit(IReadOnlyUnit unit)
	{
		foreach (TeamPosition position in _teamPositions)
		{
			if ((position.ContainsUnit) && (position.Unit == unit))
			{
				return true;
			}
		}

		return false;
	}

    /// <summary>
    /// Returns true iff this team contains specified position
    /// </summary>
    /// <param name="readOnlyPosition"></param>
    /// <returns></returns>
    public bool ContainsPosition(IReadOnlyTeamPosition readOnlyPosition)
    {
        foreach (TeamPosition position in _teamPositions)
        {
            if (position == readOnlyPosition) return true;
        }

        return false;
    }

    /// <summary>
    /// Returns position that houses the unit, or null if not found
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public TeamPosition GetPositionWithUnit(IReadOnlyUnit unit)
    {
        foreach (TeamPosition position in _teamPositions)
        {
            if ((position.ContainsUnit) && (position.Unit == unit))
            {
                return position;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns position number that houses the unit, or -1 if not found
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public int GetPositionIndexWithUnit(IReadOnlyUnit unit)
    {
        for (int i = 0; i < 3; i++)
        {
			TeamPosition position = _teamPositions[i];

            if ((position.ContainsUnit) && (position.Unit == unit))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Returns duplicate of team position array
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<TeamPosition> GetPositions()
	{
		return _teamPositions.AsReadOnly();
	}

	/// <summary>
	/// Gets unit in specified position, or null if position unoccupied
	/// </summary>
	/// <param name="positionIndex"></param>
	/// <returns></returns>
	public Unit GetUnit(int positionIndex)
	{
		TeamPosition position = _teamPositions[positionIndex];
		if (position.ContainsUnit)
		{
            return position.Unit;
        }

		return null;
	}

	public TeamPosition GetMatchingPosition(IReadOnlyTeamPosition readOnlyPosition)
	{
		foreach (TeamPosition position in _teamPositions)
		{
			if (position == readOnlyPosition) return position;
		}

		return null;
	}
	
	//Returns all units that can still move this turn
	public List<Unit> GetActiveUnits()
	{
		List<Unit> activeUnits = new List<Unit>();
		foreach (TeamPosition position in _teamPositions)
		{
			if (!position.MovedThisRound && position.ContainsUnit)
			{
				activeUnits.Add(position.Unit);
			}
		}	
		return activeUnits;
	}

    //Returns all units that can still move this turn as ReadOnlyUnit
    public List<IReadOnlyUnit> GetActiveUnitsAsReadOnly()
    {
        List<IReadOnlyUnit> activeUnits = new List<IReadOnlyUnit>();
        foreach (TeamPosition position in _teamPositions)
        {
            if (!position.MovedThisRound && position.ContainsUnit)
            {
                activeUnits.Add(position.Unit);
            }
        }
        return activeUnits;
    }

    // Returns all units adjacent to the target unit
    public List<IReadOnlyUnit> GetAdjacentReadOnlyUnits(IReadOnlyUnit target)
    {
		List<IReadOnlyUnit> adjacentUnits = new List<IReadOnlyUnit>();

        switch (GetPositionIndexWithUnit(target))
		{
			case 0:
				if (_teamPositions[1].ContainsUnit) adjacentUnits.Add(_teamPositions[1].Unit); 
				break;
            case 1:
                if (_teamPositions[0].ContainsUnit) adjacentUnits.Add(_teamPositions[0].Unit);
                if (_teamPositions[2].ContainsUnit) adjacentUnits.Add(_teamPositions[2].Unit);
				break;
            case 2:
                if (_teamPositions[1].ContainsUnit) adjacentUnits.Add(_teamPositions[1].Unit);
				break;
        }

		return adjacentUnits;
    }

    //Returns all units that can still move this turn as UnitStats
    public List<Units.UnitStats> GetActiveUnitStats()
    {
        List<Units.UnitStats> activeUnits = new List<Units.UnitStats>();
        foreach (TeamPosition position in _teamPositions)
        {
            if (!position.MovedThisRound && position.ContainsUnit)
            {
                activeUnits.Add(position.Unit.Stats);
            }
        }

        return activeUnits;
    }

    //Returns all units currently in this team
    public List<Unit> GetUnits()
	{
		List<Unit> activeUnits = new List<Unit>();
		foreach (TeamPosition position in _teamPositions)
		{
			if (position.ContainsUnit)
			{
				activeUnits.Add(position.Unit);
			}
		}
		return activeUnits;
	}

    //Returns all units currently in this team as IReadOnlyUnits
    public List<IReadOnlyUnit> GetReadOnlyUnits()
    {
        List<IReadOnlyUnit> activeUnits = new List<IReadOnlyUnit>();
        foreach (TeamPosition position in _teamPositions)
        {
            if (position.ContainsUnit)
            {
                activeUnits.Add(position.Unit);
            }
        }
        return activeUnits;
    }

	public Unit GetUnitAtPosition(int positionIndex)
	{
		if (positionIndex < 0 || positionIndex >= _teamPositions.Count)
		{
			return null; // Invalid position index
        }
		TeamPosition position = _teamPositions[positionIndex];
		return position.ContainsUnit ? position.Unit : null;
    }

	public Unit GetAnyUnit()
	{
		foreach (TeamPosition position in _teamPositions)
		{
			if (position.ContainsUnit)
			{
				return position.Unit;
			}
		}

		return null; // No unit found
    }

    public List<Units.UnitStats> GetUnitStats()
    {
        List<Units.UnitStats> activeUnits = new List<Units.UnitStats>();
        foreach (TeamPosition position in _teamPositions)
        {
            if (position.ContainsUnit)
            {
                activeUnits.Add(position.Unit.Stats);
            }
        }
        return activeUnits;
    }

	public void RetreatUnit(IReadOnlyUnit unit)
	{
        TeamPosition position = GetPositionWithUnit(unit);

        if (position != null)
        {
            position.RetreatUnit();
        }
    }

	public void KillUnit(IReadOnlyUnit unit)
	{
        TeamPosition position = GetPositionWithUnit(unit);

        if (position != null)
        {
            position.KillUnit();
        }
    }

    //
    // Move Selection
    //

    public void SelectMoveInput(CombatTeam thisTeam, CombatTeam enemyTeam) 
	{
		_moveSelector.SelectMoveInput(thisTeam, enemyTeam);
	}

	public ResultBase ExecutePayload(PayloadBase payload)
	{
		IReadOnlyUnit target = payload.Target;
		TeamPosition position = GetPositionWithUnit(target);

		return position?.ExecutePayload(payload);
	}

    public void ModifyPayload(PayloadBase payload)
    {
        // Requests intercepted in order: sender, sender's team, target's team, target
        TeamPosition senderPosition = this.GetPositionWithUnit(payload.Sender);
        senderPosition?.ModifyPayload(payload);

        foreach (TeamPosition position in _teamPositions)
        {
            if ((position.Unit != payload.Target) && (position.Unit != payload.Sender))
            {
                position.ModifyPayload(payload);
            }
        }

        TeamPosition targetPosition = this.GetPositionWithUnit(payload.Target);
        targetPosition?.ModifyPayload(payload);
    }

    public void ReactToPayloadResult(ResultBase result)
    {
		// Results intercepted in order: sender, sender's team, target's team, target
		TeamPosition senderPosition = this.GetPositionWithUnit(result.Sender);
		senderPosition?.ReactToPayloadResult(result);

        TeamPosition targetPosition = this.GetPositionWithUnit(result.Target);

        foreach (TeamPosition position in _teamPositions)
		{
			if ((position.Unit != result.Target) && (position != senderPosition) && (position!= targetPosition))
			{
				position.ReactToPayloadResult(result);
			}
		}

        if (senderPosition != targetPosition) targetPosition?.ReactToPayloadResult(result);
    }

    public bool PlayerControlled { get { return _playerControlled; } }
}