using System;
using SoulSmithMoves;
using SoulSmithModifiers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace SoulSmithObjects;
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

	private Queue<IReadOnlyUnit> _unitClearQueue;
	private int _activeTeamIndex; //Which team will be acting next
	private int _turnCount = 0;
	private int _consecutivePassedTurns = 0;
	private int _roundCount = 0;
	private bool _awaitingMoveInput;

	public CombatManager(IAssetLoadOnly assetLoader)
	{
        Initialize(assetLoader);
	}

	//
	// Initialization
	//

	private void Initialize(IAssetLoadOnly assetLoader)
	{
		InitializeTeams();
		InitializeQueues();
		InitializeUI(assetLoader);
	}

    private void InitializeUI(IAssetLoadOnly assetLoader)
    {
		SpriteFont spriteFont = assetLoader.GetFont(GameManager.UIFONTNAME);

        _combatUI = new CombatUI(spriteFont);
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
            team.UnitDeathCallEventHandler += OnUnitDeathCall;
            team.SendEffectEventHandler += ExecuteEffect;
        }
    }

    private void InitializeQueues()
    {
        if (_effectQueue == null)
        {
            _effectQueue = new EffectQueue();
			_effectQueue.ExecuteGlobalTriggerEffectEventHandler += ExecuteGlobalTriggerEffect;
        }

        AddChild(_effectQueue);

        _unitClearQueue = new Queue<IReadOnlyUnit>();
    }

    public override void Process(double delta)
	{
		if (!_awaitingMoveInput && _effectQueue.IsEmpty())
		{
			ClearUnitClearQueue();

			ReadOnlyCollection<Unit> units = GetAllActiveUnits();
			_effectQueue.OnTurnEnd();
			_effectQueue.OnTurnBegin();
			BeginTurn();
		}

        base.Process(delta);
	}

	private void ClearUnitClearQueue()
	{
		while (_unitClearQueue.Count > 0)
		{
			IReadOnlyUnit unit = _unitClearQueue.Dequeue();
            Team team = GetTeamWithUnit(unit);
			if (team != null)
			{
				if (team.PlayerControlled)
				{
					team.RemoveUnitFromCombat(unit);
					InsertUnitToInventory(unit as Unit);
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

	private Team GetTeamWithUnit(IReadOnlyUnit unit)
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

	private ReadOnlyCollection<Unit> GetAllActiveUnits()
	{
		List<Unit> units = new List<Unit>();
		foreach (Team team in _teams)
		{
			units.AddRange(team.GetActiveUnits());
		}
		return units.AsReadOnly();
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

		_effectQueue.OnRoundBegin();
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
		_consecutivePassedTurns = 0;
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

		_effectQueue.OnRoundEnd();

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
	private void OnUnitDeathCall(object sender, UnitDeathCallArgs e)
	{
		_effectQueue.OnUnitDeath(e.Killer, e.CallingUnit);
	}

	private void OnShowTargetSelectUI(object sender, ShowTargetSelectUIEventArgs e)
	{
		MoveTargetingStyle targetingStyle = e.TargetingStyle;
		IReadOnlyUnit unitSender = e.Sender;
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

	// Listens to both teams
	private void OnOfferEffectInput(object sender, EnqueueEffectInputEventArgs e)
	{
		EffectInput effectInput = e.EffectInput;
		_effectQueue.EnqueueEffect(effectInput);
	}

	// Listens to effect queue
	private void ExecuteGlobalTriggerEffect(object sender, ExecuteGlobalTriggerEffectEventArgs e)
	{
		ExecuteEffectInternal(e.EffectRequest);
	}

	// Listens to effect queue
	private void ExecuteEffect(object sender, SendEffectEventArgs e)
	{
		EffectRequest request = e.EffectRequest;

		ExecuteEffectInternal(request);
	}

	private EffectResult ExecuteEffectInternal(EffectRequest request)
	{
        Unit target = request.Target as Unit;
        if (target == null)
        {
            Trace.TraceError("CombatManager: Could not cast IReadOnlyUnit as Unit");
        }

        List<Unit> allActiveUnits = new List<Unit>(GetAllActiveUnits());
        allActiveUnits.Remove(target);
        EffectResult result = null;

        foreach (Unit unit in allActiveUnits)
        {
            ExecuteEffectForUnit(request, unit);
        }

        if (target != null)
            result = ExecuteEffectForUnit(request, target);

        _effectQueue.ResolveEffect(request, result);

        if (result != null)
            GiveEffectResultToTeams(result);

		return result;
    }

	private EffectResult ExecuteEffectForUnit(EffectRequest request, Unit unit)
	{
        Team team = GetTeamWithUnit(unit);
        EffectResult result = null;

        if (team != null) 
			result = team.ExecuteEffect(request, unit);

		return result;
    }

	private void GiveEffectResultToTeams(EffectResult result)
	{
		foreach (Team team in _teams)
		{
			team.ReceiveEffectResult(result);
		}
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