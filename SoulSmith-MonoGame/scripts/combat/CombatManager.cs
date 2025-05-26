using System;
using SoulSmithMoves;
using SoulSmithModifiers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace SoulSmithObjects;
public partial class CombatManager : CanvasObject
{
	//This team always goes first in the round
	private const int TEAMGOESFIRSTINDEX = 1;

	// Children
	private List<CombatTeam> _teams;
	private EffectQueue _effectQueue = null;
	private CombatUI _combatUI = null;

	//TODO Delete
	private bool _startingUnitsInstantiated = false;

	private EnemySpawnSelector _enemySpawnSelector = null;
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

	private void Initialize()
	{
		InitializeEnemySpawnSelector();
		InitializeTeams();
		InitializeQueues();
		InitializeUI();
	}

	private void InitializeEnemySpawnSelector()
	{
		_enemySpawnSelector = new EnemySpawnSelector();
	}

    private void InitializeUI()
    {
		SpriteFont spriteFont = MasterAssetLoader.GetFont(GameManager.UIFONTNAME);

        _combatUI = new CombatUI(spriteFont);
        AddChild(_combatUI);
    }

    private void InitializeTeams()
    {
		//TODO move
        _teams = new List<CombatTeam>();
        _teams.Add(new CombatTeam(true));
        _teams.Add(new CombatTeam(false));

        foreach (CombatTeam team in _teams)
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
    }

    public override void Process(double delta)
	{
		if (!_awaitingMoveInput && _effectQueue.IsEmpty())
		{
			ReadOnlyCollection<Unit> units = GetAllActiveUnits();
			_effectQueue.OnTurnEnd();
			_effectQueue.OnTurnBegin();
			BeginTurn();
		}

        base.Process(delta);
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

	private CombatTeam GetEnemyTeam(CombatTeam callingTeam)
	{
		foreach (CombatTeam team in _teams)
		{
			if (team != callingTeam)
			{
				return team;
			}
		}
		return null;
	}

	private CombatTeam GetComputerTeam()
	{
		foreach (CombatTeam team in _teams)
		{
			if (!team.PlayerControlled)
				return team;
		}
		return null;
	}

	private CombatTeam GetNextActiveTeam()
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

	private CombatTeam GetTeamWithUnit(IReadOnlyUnit unit)
	{
		foreach (CombatTeam team in _teams)
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
		foreach (CombatTeam team in _teams)
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
			foreach (CombatTeam team in _teams)
			{
				// TODO remove 
				string unitName = "JoyForm";
				//if (!team.PlayerControlled) unitName = "animatedScrap";
				for (int i = 0; i < 3; i++)
				{
					Unit unit = InstantiateUnitWithTemplateName(unitName);
					team.AssignUnitToPosition(unit, i);
				}
            }
			_startingUnitsInstantiated = true;
		}

		_enemySpawnSelector.UpdateRound(_roundCount);
		CheckForEnemySpawns();

		_effectQueue.OnRoundBegin();
		_turnCount = 0;
		_activeTeamIndex = TEAMGOESFIRSTINDEX;
		_roundCount++;
		
		PrepareTeamsForNewRound();
		_combatUI.Update(_roundCount);
	}

	private void PrepareTeamsForNewRound()
	{
		foreach (CombatTeam team in _teams)
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

	/// <summary>
	/// Checks and spawns enemies, returns integer representing number of enemies spawned
	/// </summary>
	/// <returns></returns>
	private int CheckForEnemySpawns()
	{
		CombatTeam enemyTeam = GetComputerTeam();
		var positions = enemyTeam.GetPositions();
		int enemiesSpawned = 0;

		for (int i = 0; i < positions.Count; i++)
		{
			TeamPosition position = positions[i];
            if (!position.ContainsUnit)
            {
				string unitTemplateName = _enemySpawnSelector.Select();
				if (!(unitTemplateName is null))
					SpawnUnitAtPosition(enemyTeam, i, unitTemplateName);
				enemiesSpawned++;
            }
        }

		return enemiesSpawned;
	}

	private void SpawnUnitAtPosition(CombatTeam team, int positionIndex, string unitTemplateName)
	{
		Unit unit = MasterAssetLoader.InstantiateUnit(unitTemplateName);
		//TODO make effect
		team.AssignUnitToPosition(unit, positionIndex);
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
		_effectQueue.OnRoundEnd();

		RoundEndEventArgs e = new RoundEndEventArgs();

		RoundEndEventHandler(this, e);
	}

	//Listens to both teams
	private void OnOfferMoveAndUser(object sender, MoveButtonPressedEventArgs args)
	{
		foreach (CombatTeam team in _teams)
		{
			team.HideMoveSelectUI();
		}
		ActiveTeam.AssignMoveAndUserToMSL(args);
	}

	//Listens to both teams
	private void OnOfferTarget(object sender, TargetButtonPressedEventArgs args)
	{
		Unit target = args.Target;

		foreach (CombatTeam team in _teams)
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
		CombatTeam senderTeam = sender as CombatTeam;

		if (senderTeam == null)
		{
			return;
		}

		CombatTeam showingTeam;

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
		Unit target = null;

		if (request.Target != null)
		{
            target = (Unit)request.Target;
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
		{
			GiveEffectResultToTeams(result);

            if (result.TriggerApplied == EffectTrigger.OnUnitDeath)
            {
                KillUnit(result.Target);
            }
        }

		return result;
    }

	private void KillUnit(IReadOnlyUnit unit)
	{
		if (unit == null) return;

		if (!unit.InCombat) return;

        CombatTeam team = GetTeamWithUnit(unit);

        team.RemoveUnitFromCombat(unit);

        if (team.PlayerControlled)
        {
            InsertUnitToInventory((Unit)unit);
        } 
    }

    private EffectResult ExecuteEffectForUnit(EffectRequest request, Unit unit)
	{
        CombatTeam team = GetTeamWithUnit(unit);
        EffectResult result = null;

        if (team != null) 
			result = team.ExecuteEffect(request, unit);

		return result;
    }

	private void GiveEffectResultToTeams(EffectResult result)
	{
		foreach (CombatTeam team in _teams)
		{
			team.ReceiveEffectResult(result);
		}
	}

	private bool IsTurnOver()
	{
		return (_consecutivePassedTurns >= _teams.Count);
	}

	public CombatTeam ActiveTeam { get { return _teams[_activeTeamIndex]; } }	
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