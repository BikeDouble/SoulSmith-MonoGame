using System;
using SoulSmithMoves;
using System.Collections.Generic;

public partial class CombatManager : CanvasItem
{
	//This team always goes first in the round
	private const int TEAMGOESFIRSTINDEX = 1;

	// Children
	private List<Team> _teams;
	private EffectQueue _effectQueue = null;
	private CombatUI _combatUI = null;

	//TODO Delete
	private bool _startingUnitsInstantiated = false;

	private Queue<Unit> _unitClearQueue;
	private int _activeTeamIndex; //Which team will be acting next
	private int _turnCount = 0;
	private int _consecutivePassedTurns = 0;
	private int _roundCount = 0;
	private bool _awaitingMoveInput;

	public CombatManager()
	{
        Initialize();
	}

	//
	// Initialization
	//

	public void Initialize()
	{
		InitializeTeams();
		InitializeQueues();
		InitializeUI();
	}

    private void InitializeUI()
    {
        _combatUI = new CombatUI();
        AddChild(_combatUI);
    }

    private void InitializeTeams()
    {
		//TODO move
        _teams = new List<Team>();
        _teams.Add(new Team(true));
        _teams.Add(new Team(false));

        foreach (Team team in _teams)
        {
            AddChild(team);
            team.OfferCompleteMoveInputEventHandler += OnOfferCompleteMoveInput;
            team.OfferPassTurnEventHandler += OnOfferPassTurn;
            team.OfferMoveAndUserEventHandler += OnOfferMoveAndUser;
            team.OfferTargetEventHandler += OnOfferTarget;
            team.EnqueueEffectInputEventHandler += OnOfferEffectInput;
            team.ShowMoveSelectUIEventHandler += OnShowMoveSelectUI;
            team.ShowTargetSelectUIEventHandler += OnShowTargetSelectUI;
            team.UnitZeroHPEventHandler += OnUnitZeroHP;
            team.SendEffectEventHandler += ExecuteEffect;
        }
    }

    private void InitializeQueues()
    {
        if (_effectQueue == null)
        {
            _effectQueue = new EffectQueue();
        }

        AddChild(_effectQueue);

        _unitClearQueue = new Queue<Unit>();
    }

    public override void Process(double delta)
	{
		if (!_awaitingMoveInput && _effectQueue.IsEmpty())
		{
			ClearUnitClearQueue();
			BeginTurn();
		}

		base.Process(delta);
	}

	private void ClearUnitClearQueue()
	{
		while (_unitClearQueue.Count > 0)
		{
			Unit unit = _unitClearQueue.Dequeue();
            Team team = GetTeamWithUnit(unit);
			if (team != null)
			{
				if (team.PlayerControlled)
				{
					team.RemoveUnitFromCombat(unit);
					InsertUnitToInventory(unit);
				}
				else
				{
					team.DeleteUnit(unit);
				}
			}
		}
	}

	public event EventHandler<OfferUnitToInventoryEventArgs> OfferUnitToInventoryEventHandler;
	private void InsertUnitToInventory(Unit unit)
	{
		OfferUnitToInventoryEventArgs e = new();
		e.Unit = unit;
		OfferUnitToInventoryEventHandler(this, e);
	}

	//
	// Getters
	//

	private Team GetEnemyTeam(Team callingTeam)
	{
		foreach (Team team in _teams)
		{
			if (team != callingTeam)
			{
				return team;
			}
		}
		return null;
	}

	private Team GetNextActiveTeam()
	{
		return _teams[GetNextActiveTeamIndex()];
	}

	private int GetNextActiveTeamIndex()
	{
		int nextIndex = _activeTeamIndex + 1;

		if (nextIndex >= _teams.Count)
		{
			nextIndex = 0;
		}

		return nextIndex;
	}

	private Team GetTeamWithUnit(Unit unit)
	{
		foreach (Team team in _teams)
		{
			if (team.ContainsUnit(unit))
			{
				return team;
			}
		}

		return null;
	}

	private List<Unit> GetAllActiveUnits()
	{
		List<Unit> units = new List<Unit>();
		foreach (Team team in _teams)
		{
			units.AddRange(team.GetActiveUnits());
		}
		return units;
	}

	//
	// Combat Processing
	//

	
	
	//
	// Round Processing
	//
	
	public void BeginRound() //AKA round end, there's not really a difference
	{
		if (!_startingUnitsInstantiated)
		{
			foreach (Team team in _teams)
			{
                // TODO remove 
                Unit unit = InstantiateUnitWithTemplateName("JoyForm");
                team.AssignUnitToPosition(unit, 0);
            }
			_startingUnitsInstantiated = true;
		}

		_effectQueue.OnRoundBegin(GetAllActiveUnits());
		_turnCount = 0;
		_activeTeamIndex = TEAMGOESFIRSTINDEX;
		_roundCount++;
		
		PrepareTeamsForNewRound();
		_combatUI.Update(_roundCount);
	}

	private void PrepareTeamsForNewRound()
	{
		foreach (Team team in _teams)
		{
			team.OnBeginRound();
		}
	}

	//
	// Unit Management
	//

	public event EventHandler<UnitInstantiationEventArgs> UnitInstantiationEventHandler;

	private Unit InstantiateUnitWithTemplateName(string unitTemplateName)
	{
		UnitInstantiationEventArgs e = new UnitInstantiationEventArgs();

		e.UnitTemplateName = unitTemplateName;

		UnitInstantiationEventHandler(this, e);

		return e.Unit;
	}

	//
	// Turn Processing
	//

	public void BeginTurn()
	{
		CycleActiveTeam();
		_turnCount++;
		_awaitingMoveInput = true;
		ActiveTeam.SelectMoveInput(ActiveTeam, GetEnemyTeam(ActiveTeam));
	}
	
	private void CycleActiveTeam()
	{
		_activeTeamIndex = GetNextActiveTeamIndex();
	}

	//Listens to both teams
	public void OnOfferCompleteMoveInput(object sender, OfferCompleteMoveInputEventArgs e)
	{
		_awaitingMoveInput = false;
		MoveInput moveInput = e.MoveInput;
		_effectQueue.EnqueueMove(moveInput);
		
		//BeginTurn();
	}

	//Listens to both teams
	public void OnOfferPassTurn(object sender, OfferPassTurnEventArgs e)
	{
		_awaitingMoveInput = false;
		_consecutivePassedTurns++;

		if (IsTurnOver())
		{
			_consecutivePassedTurns = 0;
			EndRound();
		}
	}

	public event EventHandler<RoundEndEventArgs> RoundEndEventHandler;

	private void EndRound()
	{
		ClearUnitClearQueue();

		_effectQueue.OnRoundEnd(GetAllActiveUnits());

		RoundEndEventArgs e = new RoundEndEventArgs();

		RoundEndEventHandler(this, e);
	}

	//Listens to both teams
	private void OnOfferMoveAndUser(object sender, MoveButtonPressedEventArgs args)
	{
		foreach (Team team in _teams)
		{
			team.HideMoveSelectUI();
		}
		ActiveTeam.AssignMoveAndUserToMSL(args);
	}

	//Listens to both teams
	private void OnOfferTarget(object sender, TargetButtonPressedEventArgs args)
	{
		Unit target = args.Target;

		foreach (Team team in _teams)
		{
			team.HideTargetSelectUI();
		}
		ActiveTeam.GiveTargetToMSL(target);
	}

	//Listens to both teams
	private void OnShowMoveSelectUI(object sender, ShowMoveSelectUIEventArgs e)
	{
		ActiveTeam.ShowMoveSelectUIOrder();
	}

	//Listens to both teams
	private void OnUnitZeroHP(object sender, ZeroHPEventArgs e)
	{
		Unit unit = e.Unit;
		_unitClearQueue.Enqueue(unit);
	}

	private void OnShowTargetSelectUI(object sender, ShowTargetSelectUIEventArgs e)
	{
		MoveTargetingStyle targetingStyle = e.TargetingStyle;
		Unit unitSender = e.Sender;
		List<int> positions;
		Team senderTeam = sender as Team;

		if (senderTeam == null)
		{
			return;
		}

		Team showingTeam;

		switch (targetingStyle)
		{
			case MoveTargetingStyle.AllyOrSelf:
				positions = new List<int>{0, 1, 2};
				showingTeam = senderTeam;
				break;
			case MoveTargetingStyle.Ally:
				positions = new List<int>{0, 1, 2};
                int userPosition = GetTeamWithUnit(unitSender).GetPositionIndexWithUnit(unitSender);
                positions.Remove(userPosition);
				showingTeam = senderTeam;
				break;
			case MoveTargetingStyle.Enemy:
				positions = new List<int>{0, 1, 2};
				showingTeam = GetEnemyTeam(senderTeam);
				break;
			default:
				positions = new List<int>();
				showingTeam = null;
				break;
		}

		showingTeam.ShowTargetSelectUIOrder(positions);
	}

	//Listens to both teams
	private void OnOfferEffectInput(object sender, EnqueueEffectInputEventArgs e)
	{
		EffectInput effectInput = e.EffectInput;
		_effectQueue.EnqueueEffect(effectInput);
	}

	private void ExecuteEffect(object sender, SendEffectEventArgs e)
	{
		EffectRequest request = e.EffectRequest;
		Unit target = request.Target;
		Team targetTeam = GetTeamWithUnit(target);
		EffectResult result;
		if (targetTeam != null)
		{
			result = targetTeam.ExecuteEffect(request);
		}
		else
		{
			result = new EffectResult();
		}

		//TODO REDO THIS ENTIRE SYSTEM WHY DID I DO THIS ???? D:
		_effectQueue.ResolveEffect(request, result);
	}

	private bool IsTurnOver()
	{
		return (_consecutivePassedTurns >= _teams.Count);
	}

	public Team ActiveTeam { get { return _teams[_activeTeamIndex]; } }	
}

public class OfferUnitToInventoryEventArgs : EventArgs
{
	public Unit Unit;
}

public class UnitInstantiationEventArgs : EventArgs
{
	public string UnitTemplateName;
	public Unit Unit = null;
}

public class RoundEndEventArgs : EventArgs
{

}