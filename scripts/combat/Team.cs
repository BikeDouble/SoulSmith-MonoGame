using System;
using SoulSmithMoves;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using SoulSmith_MonoGame;

public partial class Team : CanvasItem
{

	private MoveSelector _moveSelector = null;
	private bool _playerControlled;
	private List<TeamPosition> _teamPositions = new List<TeamPosition>();

	public Team(bool playerControlled)
	{
		_playerControlled = playerControlled;

		if (_playerControlled)
		{
			_moveSelector = new MoveSelector_PlayerInput();
		}
		else
		{
			_moveSelector = new MoveSelector_Random();
		}

		Initialize();
	}

	public Team()
	{
        _moveSelector = new MoveSelector_Random();
        Initialize();
    }

	private void Initialize()
	{
		InitializeMoveSelector();
		InitializePositions();
	}

	private void InitializeMoveSelector()
	{
		if (_moveSelector == null) 
		{
			_moveSelector = new MoveSelector_Random();
		}

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
				_teamPositions.Add(new TeamPosition(100, Game1.WINDOWHEIGHT / 4));
				_teamPositions.Add(new TeamPosition(200, Game1.WINDOWHEIGHT / 2));
				_teamPositions.Add(new TeamPosition(100, 3 * Game1.WINDOWHEIGHT / 4));
			}
			else
			{
				_teamPositions.Add(new TeamPosition(Game1.WINDOWLENGTH - 100, Game1.WINDOWHEIGHT / 4));
				_teamPositions.Add(new TeamPosition(Game1.WINDOWLENGTH - 200, Game1.WINDOWHEIGHT / 2));
				_teamPositions.Add(new TeamPosition(Game1.WINDOWLENGTH - 100, 3 * Game1.WINDOWHEIGHT / 4));
			}
        }

		foreach (TeamPosition position in _teamPositions)
		{
			AddChild(position);
			position.PlayerControlled = _playerControlled;
			position.OfferMoveAndUserEventHandler += OnOfferMoveAndUser;
			position.OfferTargetEventHandler += OnOfferTarget;
			position.EnqueueEffectInputEventHandler += EnqueueEffectInput;
			position.UnitZeroHPEventHandler += OnUnitZeroHP;
			position.SendEffectEventHandler += SendEffect;
		}
	}

    public event EventHandler<EnqueueEffectInputEventArgs> EnqueueEffectInputEventHandler;

    private void EnqueueEffectInput(object sender, EnqueueEffectInputEventArgs e)
    {
        EnqueueEffectInputEventHandler(this, e);
    }

	public event EventHandler<SendEffectEventArgs> SendEffectEventHandler;

	//Connected to team positions, sends up
	private void SendEffect(object sender, SendEffectEventArgs e)
	{
		SendEffectEventHandler(this, e);
	}

    public void DeleteUnit(Unit unit)
	{
		TeamPosition position = GetPositionWithUnit(unit);

		if (position == null)
		{
			return;
		}

		position.DeleteUnit();
	}

	public void AssignUnitToPosition(Unit unit, int positionIndex)
	{
		if (unit == null)
		{
			Trace.TraceError("Unit assigned to team is null.");
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

	public event EventHandler<ZeroHPEventArgs> UnitZeroHPEventHandler;

	private void OnUnitZeroHP(object sender, ZeroHPEventArgs e)
	{
		UnitZeroHPEventHandler(this, e);
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
	public bool ContainsUnit(Unit unit)
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
    public TeamPosition GetPositionWithUnit(Unit unit)
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
    public int GetPositionIndexWithUnit(Unit unit)
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

    //Returns all units that can still move this turn as UnitStats
    public List<UnitStats> GetActiveUnitStats()
    {
        List<UnitStats> activeUnits = new List<UnitStats>();
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

    public List<UnitStats> GetUnitStats()
    {
        List<UnitStats> activeUnits = new List<UnitStats>();
        foreach (TeamPosition position in _teamPositions)
        {
            if (position.ContainsUnit)
            {
                activeUnits.Add(position.Unit.Stats);
            }
        }
        return activeUnits;
    }

	public void RemoveUnitFromCombat(Unit unit)
	{
		TeamPosition position = GetPositionWithUnit(unit);

		if (position != null)
		{
            position.RemoveUnitFromCombat();
        }
	}
	
	//
	// Move Selection
	//
	
	public void SelectMoveInput(Team thisTeam, Team enemyTeam) 
	{
		_moveSelector.SelectMoveInput(thisTeam, enemyTeam);
	}

    public EffectResult ExecuteEffect(EffectRequest request)
    {
		TeamPosition targetPosition = GetPositionWithUnit(request.Target);
		if (targetPosition != null)
		{
			return targetPosition.ExecuteEffect(request);
		}
		else
		{
			return new EffectResult();
		}
    }

    public bool PlayerControlled { get { return _playerControlled; } }
}