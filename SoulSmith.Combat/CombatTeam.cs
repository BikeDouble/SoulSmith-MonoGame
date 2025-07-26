using System.Collections.ObjectModel;
using System.Diagnostics;
using SoulSmith.Object.Canvas;
using SoulSmith.Drawing;
using SoulSmith.Battle;
using SoulSmith.Battle.Moves;
using SoulSmith.Units;
using SoulSmith.Battle.Effects;

namespace SoulSmith.Combat;
public partial class CombatTeam : CanvasObject, IReadOnlyCombatTeam
{
	public const int BACKUNITSDISTANCEFROMSCREENEDGE = 250;
	public const int FRONTUNITSDISTANCEFROMSCREENEDGE = 400;
	public const int YOFFSET = 50;

    private IMoveSelector _moveSelector = null;
	private bool _playerControlled;
	private List<TeamPosition> _teamPositions = new List<TeamPosition>();

	public CombatTeam(IMoveSelector moveSelector) : base(new Core.Position(0, YOFFSET))
	{
		_moveSelector = moveSelector;

		_playerControlled = _moveSelector.PlayerControlled;

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
		_moveSelector.OfferPassTurnEventHandler += OnOfferPassTurn;
	}

    private void InitializePositions()
	{
		if (_teamPositions.Count == 0)
		{
			if (_playerControlled)
			{
				_teamPositions.Add(new TeamPosition(BACKUNITSDISTANCEFROMSCREENEDGE, Window.WINDOWHEIGHT / 4));
				_teamPositions.Add(new TeamPosition(FRONTUNITSDISTANCEFROMSCREENEDGE, Window.WINDOWHEIGHT / 2));
				_teamPositions.Add(new TeamPosition(BACKUNITSDISTANCEFROMSCREENEDGE, 3 * Window.WINDOWHEIGHT / 4));
			}
			else
			{
				_teamPositions.Add(new TeamPosition(Window.WINDOWLENGTH - BACKUNITSDISTANCEFROMSCREENEDGE, Window.WINDOWHEIGHT / 4, -1, 1));
				_teamPositions.Add(new TeamPosition(Window.WINDOWLENGTH - FRONTUNITSDISTANCEFROMSCREENEDGE, Window.WINDOWHEIGHT / 2, -1, 1));
				_teamPositions.Add(new TeamPosition(Window.WINDOWLENGTH - BACKUNITSDISTANCEFROMSCREENEDGE, 3 * Window.WINDOWHEIGHT / 4, -1, 1));
			}
        }

		foreach (TeamPosition position in _teamPositions)
		{
			AddChild(position);
			position.OfferMoveAndUserEventHandler += OnOfferMoveAndUser;
			position.OfferTargetEventHandler += OnOfferTarget;
			position.EnqueueEffectInputEventHandler += EnqueueEffectInput;
			position.UnitDeathCallEventHandler += OnUnitDeathCall;
			position.UnitRetreatCallEventHandler += OnUnitRetreatCall;
        }
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
			Trace.TraceError("UnitSprite assigned to team is null.");
			return;
		}

		TeamPosition position = _teamPositions[positionIndex];

		if (position.ContainsUnit)
		{
			Trace.TraceError("Attempted to assign unit to position that already contains unit.");
			return;
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

	public void ShowMoveSelectUIOrder()
	{
		foreach (TeamPosition position in _teamPositions)
		{
			position.ShowMoveSelectUI();
		}
	}

	public void ShowTargetSelectUIOrder(List<int> positionNumbers)
	{
		foreach (int positionNumber in positionNumbers)
		{
			_teamPositions[positionNumber].ShowTargetSelectUI();
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
	
	//Returns all units that can still move this turn
	public ReadOnlyCollection<Unit> GetActiveUnits()
	{
		List<Unit> activeUnits = new List<Unit>();
		foreach (TeamPosition position in _teamPositions)
		{
			if (!position.MovedThisRound && position.ContainsUnit)
			{
				activeUnits.Add(position.Unit);
			}
		}	
		return activeUnits.AsReadOnly();
	}

    //Returns all units that can still move this turn as ReadOnlyUnit
    public ReadOnlyCollection<IReadOnlyUnit> GetActiveUnitsAsReadOnly()
    {
        List<IReadOnlyUnit> activeUnits = new List<IReadOnlyUnit>();
        foreach (TeamPosition position in _teamPositions)
        {
            if (!position.MovedThisRound && position.ContainsUnit)
            {
                activeUnits.Add(position.Unit);
            }
        }
        return activeUnits.AsReadOnly();
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

	public EffectResult ExecuteEffectRequest(EffectRequest request)
	{
		IReadOnlyUnit target = request.Target;
		TeamPosition position = GetPositionWithUnit(target);

		return position?.ExecuteEffectRequest(request);
	}

    public void ModifyEffectRequest(EffectRequest request)
    {
        // Requests intercepted in order: sender, sender's team, target's team, target
        TeamPosition senderPosition = this.GetPositionWithUnit(request.Sender);
        senderPosition?.ModifyEffectRequest(request);

        foreach (TeamPosition position in _teamPositions)
        {
            if ((position.Unit != request.Target) && (position.Unit != request.Sender))
            {
                position.ModifyEffectRequest(request);
            }
        }

        TeamPosition targetPosition = this.GetPositionWithUnit(request.Target);
        targetPosition?.ModifyEffectRequest(request);
    }

    public void ReactToEffectResult(EffectResult result)
    {
		// Results intercepted in order: sender, sender's team, target's team, target
		TeamPosition senderPosition = this.GetPositionWithUnit(result.Sender);
		senderPosition?.ReactToEffectResult(result);

        foreach (TeamPosition position in _teamPositions)
		{
			if ((position.Unit != result.Target) && (position.Unit != result.Sender))
			{
				position.ReactToEffectResult(result);
			}
		}

        TeamPosition targetPosition = this.GetPositionWithUnit(result.Target);
        targetPosition?.ReactToEffectResult(result);
    }

    public bool PlayerControlled { get { return _playerControlled; } }
}