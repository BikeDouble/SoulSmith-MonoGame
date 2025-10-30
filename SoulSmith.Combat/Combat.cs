using System;
using SoulSmith.Object.Canvas;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Units;
using SoulSmith.Battle;
using SoulSmith.Battle.Moves;
using SoulSmith.Asset;
using SoulSmith.Templates;
using SoulSmith.Collections;
using SoulSmith.Battle.Effects;
using SoulSmith.Drawing;
using SoulSmith.Battle.Effects.Trigger;
using SoulSmith.Battle.Effects.Payloads;
using SoulSmith.Battle.Effects.Results;

namespace SoulSmith.Combats;
public class Combat : CanvasObject, IReadOnlyCombat
{
	//This team always goes first in the round
	private const int TEAMGOESFIRSTINDEX = 1;

	// Children
	private List<CombatTeam> _teams;
	private EffectQueue _effectQueue = null;
	private CombatUI _combatUI = null;

	//TODO Delete
	private bool _startingUnitsInstantiated = false;

	public static MasterSpawnList MasterSpawnList = null;
	private int _activeTeamIndex; //Which team will be acting next
	private int _turnCount = 0;
	private int _consecutivePassedTurns = 0;
	private int _roundCount = 0;
	private int _currentArea = 1;
	private bool _awaitingMoveInput;
	private List<Unit> _retreatingUnits = new List<Unit>();

    public Combat(IMoveSelector playerMoveSelector, IMoveSelector enemyMoveSelector)
	{
        Initialize(playerMoveSelector, enemyMoveSelector);
	}

	//
	// Initialization
	//

	private void Initialize(IMoveSelector playerMoveSelector, IMoveSelector enemyMoveSelector)
	{
		InitializeMasterSpawnList();
		InitializeTeams(playerMoveSelector, enemyMoveSelector);
		InitializeQueues();
		InitializeUI();
	}

	private void InitializeMasterSpawnList()
	{
		Dictionary<(int Difficulty, int Area), SoulSmithWeightedList<string>> passiveSpawnLists = new Dictionary<(int Difficulty, int Area), SoulSmithWeightedList<string>>
		{
			[(1, 1)] = AssetManager.Instance.GetSpawnList("SpawnLists/Area1/Passive1")
		};

		Dictionary<int, SoulSmithWeightedList<string>> eliteSpawnLists = new Dictionary<int, SoulSmithWeightedList<string>>
		{

		};

        Dictionary<int, SoulSmithWeightedList<string>> bossSpawnLists = new Dictionary<int, SoulSmithWeightedList<string>>
        {

        };

        MasterSpawnList = new MasterSpawnList(passiveSpawnLists, eliteSpawnLists, bossSpawnLists);
	}

    private void InitializeUI()
    {
		SpriteFont spriteFont = null;// MasterAssetLoader.GetFont(GameManager.UIFONTNAME); TODO fix fonts

        _combatUI = new CombatUI();
        AddChild(_combatUI);
    }

    private void InitializeTeams(IMoveSelector playerMoveSelector, IMoveSelector enemyMoveSelector)
    {
		//TODO move
        _teams = new List<CombatTeam>();
        _teams.Add(new CombatTeam(playerMoveSelector, true));
        _teams.Add(new CombatTeam(enemyMoveSelector, false));

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
			team.ShowDeployUnitUIEventHandler += OnShowDeployUnitUI;
            team.UnitDeathCallEventHandler += OnUnitDeathCall;
			team.UnitRetreatCallEventHandler += OnUnitRetreatCall;
			team.DeployUnitButtonPressedEventHandler += OnDeployUnitButtonPressed;
        }
    }

    private void InitializeQueues()
    {
        if (_effectQueue == null)
        {
            _effectQueue = new EffectQueue(this);
			_effectQueue.ExecuteEffectEventHandler += ExecuteEffect;
        }

        AddChild(_effectQueue);
    }

    public override void Process(double delta)
	{
		if (!_awaitingMoveInput && _effectQueue.IsEmpty())
		{
			foreach (Unit unit in _retreatingUnits) RetreatUnit(unit);
            _retreatingUnits.Clear();
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

	private CombatTeam GetEnemyTeam(IReadOnlyCombatTeam callingTeam)
	{
		if (callingTeam == null) return null;

		foreach (CombatTeam team in _teams)
		{
			if (team != callingTeam)
			{
				return team;
			}
		}
		return null;
	}

	public IReadOnlyCombatTeam GetEnemyReadOnlyTeam(IReadOnlyCombatTeam callingTeam)
	{
		return GetEnemyTeam(callingTeam);
    }

    public IReadOnlyCombatTeam GetEnemyReadOnlyTeam(IReadOnlyUnit callingUnit)
    {
		return GetEnemyTeam(GetTeamWithUnit(callingUnit));
    }

	private CombatTeam GetPlayerTeam()
	{
        foreach (CombatTeam team in _teams)
        {
            if (team.PlayerControlled)
                return team;
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

    private CombatTeam GetTeamWithPosition(IReadOnlyTeamPosition position)
    {
        foreach (CombatTeam team in _teams)
        {
            if (team.ContainsPosition(position))
            {
                return team;
            }
        }

        return null;
    }

    public IReadOnlyCombatTeam GetReadOnlyTeamWithUnit(IReadOnlyUnit unit)
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

    // Returns all units in combat that can still move this round
    public ReadOnlyCollection<IReadOnlyUnit> GetAllActiveUnitsAsReadOnly()
	{
        List<IReadOnlyUnit> units = new List<IReadOnlyUnit>();
        foreach (CombatTeam team in _teams)
        {
            units.AddRange(team.GetActiveUnitsAsReadOnly());
        }
        return units.AsReadOnly();
    }

	public ReadOnlyCollection<IReadOnlyUnit> GetAllReadOnlyUnits()
	{
		List<IReadOnlyUnit> units = new List<IReadOnlyUnit>();
		foreach (CombatTeam team in _teams)
		{
			units.AddRange(team.GetReadOnlyUnits());
		}
		return units.AsReadOnly();
    }

	public IReadOnlyUnit GetReadOnlyUnitAcrossFrom(IReadOnlyUnit unit)
	{
		if (unit == null) return null;
		CombatTeam team = GetTeamWithUnit(unit);
		if (team == null) return null;
        int unitPositionIndex = team.GetPositionIndexWithUnit(unit);
		CombatTeam acrossTeam = GetEnemyTeam(team);
		if (acrossTeam == null) return null;
		return acrossTeam.GetUnitAtPosition(unitPositionIndex);
    }

    // Gets any enemy unit that is currently in combat, NOT RANDOM
    public IReadOnlyUnit GetAnyEnemyReadOnlyUnit(IReadOnlyUnit unit)
	{
		if (unit == null) return null;
		CombatTeam team = GetTeamWithUnit(unit);
		if (team == null) return null;
		CombatTeam enemyTeam = GetEnemyTeam(team);
		if (enemyTeam == null) return null;
		return enemyTeam.GetAnyUnit();
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
				string joyUnitKey = "Units/Forms/Single/Joy";
				string angerUnitKey = "Units/Forms/Single/Anger";
				string joyAngerUnitKey = "Units/Forms/Double/JoyAnger";
				//if (!team.PlayerControlled) unitName = "animatedScrap";
				for (int i = 0; i < 3; i++)
				{
					string unitKey = i switch
					{
						0 => joyUnitKey,
                        1 => joyAngerUnitKey,
                        2 => angerUnitKey,
                        _ => throw new ArgumentOutOfRangeException(nameof(i), i, "Invalid index")
                    };
                    IAssetWrapper<UnitTemplate> templateAsset = AssetManager.Instance.GetUnitTemplate<UnitTemplate>(unitKey);
                    if (templateAsset == null) throw new ArgumentNullException(nameof(templateAsset));
                    Unit unit = new Unit(templateAsset.Value);
					team.AssignUnitToPosition(unit, i);
				}
            }
			_startingUnitsInstantiated = true;
		}

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
				string unitTemplateName = MasterSpawnList.SelectPassiveSpawn(1, _currentArea); //TODO implement difficulty
				if (!(unitTemplateName is null))
					SpawnUnitAtPosition(enemyTeam, i, unitTemplateName);
				enemiesSpawned++;
            }
        }

		return enemiesSpawned;
	}

	private void SpawnUnitAtPosition(CombatTeam team, int positionIndex, string unitTemplateName)
	{
		//Unit unit = new Unit //TODO implement UnitTemplate
		//TODO make effect
		//team.AssignUnitToPosition(unit, positionIndex);
	}

	public void DeployUnitAtPosition(IReadOnlyTeamPosition position, Unit unit) //TODO implement deploy phase
	{
		CombatTeam team = GetTeamWithPosition(position);

		team.AssignUnitToPosition(unit, position);
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

	public event EventHandler<GetUnitsInInventoryEventArgs> GetUnitsInInventoryEventHandler;

	private List<IReadOnlyUnit> GetUnitsInInventory()
	{
		GetUnitsInInventoryEventArgs e = new GetUnitsInInventoryEventArgs();

		GetUnitsInInventoryEventHandler?.Invoke(this, e);

		return e.UnitsInInventory;
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
		ActiveTeam.ShowMoveSelectUI();
	}

	//Listens to both teams
	private void OnShowDeployUnitUI(object sender, ShowDeployUnitUIEventArgs e)
	{
        if (ActiveTeam.PlayerControlled)
        {
            List<IReadOnlyUnit> unitsInInventory = GetUnitsInInventory();
            if (unitsInInventory != null && unitsInInventory.Count > 0)
            {
                ActiveTeam.ShowDeployUnitUI();
            }
        }
    }

	//Listens to both teams
	private void OnUnitDeathCall(object sender, UnitDeathCallArgs e) 
	{
		_effectQueue.OnUnitDeath(e.Killer, e.CallingUnit, e.KillingEffectResult);
		GetTeamWithUnit(e.CallingUnit).PlayDeathAnimationForUnit(e.CallingUnit);
    }

	private void OnUnitRetreatCall(object sender, UnitRetreatCallArgs e)
	{
		if (e.FromRetrieveButton)
		{
            _consecutivePassedTurns = 0;
            _awaitingMoveInput = false;
        }
		_effectQueue.OnUnitRetreat(e.RetreatingUnit);
        foreach (CombatTeam team in _teams)
        {
            team.HideMoveSelectUI();
        }
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
			case MoveTargetingStyle.Self:
                userPosition = GetTeamWithUnit(unitSender).GetPositionIndexWithUnit(unitSender);
                positions = new List<int>{userPosition};
                showingTeam = senderTeam;
				break;
			default:
				positions = new List<int>();
				showingTeam = null;
				break;
		}

		showingTeam.ShowTargetSelectUI(positions);
	}

	// Listens to both teams
	private void OnOfferEffectInput(object sender, EnqueueEffectInputEventArgs e)
	{
		EffectInput effectInput = e.EffectInput;
        _effectQueue.EnqueueEffect(effectInput);
	}

    public EventHandler<DeployUnitButtonPressedEventArgs> DeployUnitButtonPressedEventHandler;

	// Listenst to both teams
    private void OnDeployUnitButtonPressed(object sender, DeployUnitButtonPressedEventArgs e)
    {
        DeployUnitButtonPressedEventHandler?.Invoke(this, e);
    }

    // Listens to effect queue
    private void ExecuteEffect(object sender, ExecuteEffectEventArgs e)
	{
		PayloadBase request = e.Payload;

		ProcessPayload(request);
	}
	
	private void ProcessPayload(PayloadBase payload)
	{
		LetTeamsModifyPayload(payload);
        ResultBase result = ExecutePayload(payload);
		if (result != null)
		{
			_effectQueue.ResolveEffect(payload, result);
			ReactToEffectResult(result);
		}
    }

	private void ReactToEffectResult(ResultBase result)
	{
        if (result != null)
        {
            GiveEffectResultToTeams(result);

			if (result is TriggerResult triggerResult)
			{
				if (triggerResult.Trigger == CombatTrigger.OnUnitDeath)
				{
					KillUnit(triggerResult.Target);
				}
				else if (triggerResult.Trigger == CombatTrigger.OnUnitRetreat)
				{
					_retreatingUnits.Add((Unit)triggerResult.Target);
				}
			}
        }
    }

	private void LetTeamsModifyPayload(PayloadBase payload)
	{
        if (payload is TriggerPayload triggerPayload) // Combat trigger payloads should not be modified
        {
			return;
        }

        Unit target = null;
		Unit sender = null;

		if (payload.Target == null) throw new ArgumentNullException(nameof(payload.Target), "Effect payload must have a target.");

        target = (Unit)payload.Target; //TODO type safety

		if (payload.Sender != null)
		{
			sender = (Unit)payload.Sender;
		}

        // Requests intercepted in order: sender, sender's team (may include target), other team, target*
        // if sender is null, target's team is used first instead
        CombatTeam firstTeam = null;

		if (sender == null)
		{
			firstTeam = GetTeamWithUnit(target);
		}
		else
		{
            firstTeam = GetTeamWithUnit(sender);
        }

		if (firstTeam == null) firstTeam = GetPlayerTeam();

		CombatTeam otherTeam = GetEnemyTeam(firstTeam);

		if (firstTeam == null || otherTeam == null) throw new ArgumentException("Invalid teams for effect execution.");

		firstTeam.ModifyPayload(payload);
		otherTeam.ModifyPayload(payload);
    }

	private ResultBase ExecutePayload(PayloadBase payload)
	{
		if (payload is TriggerPayload triggerPayload)
		{
            // Trigger payloads should not be executed, but rather returned as a result and used exclusively for triggering other effects
            return new TriggerResult(payload.Sender, payload.Target, triggerPayload.Trigger, payload, payload.Originator); 
        }

		if (payload is AOEPayloadBase aOEPayload)
		{
            // AOE payloads require special handling
            return ExecuteAOEPayload(aOEPayload);
        }

		CombatTeam targetTeam = GetTeamWithUnit(payload.Target);
		if (targetTeam == null) return null;//throw new ArgumentException("Target team not found for payload execution.");
        ResultBase result = targetTeam.ExecutePayload(payload);

        return result;
    }

	private ResultBase ExecuteAOEPayload(AOEPayloadBase aOEPayload)
	{
        List<ResultBase> results = new List<ResultBase>();
        foreach (IReadOnlyUnit target in aOEPayload.AllTargets)
        {
            PayloadBase singleTargetPayload = aOEPayload.GetTargetSpecificPayload(target);
            ResultBase singleTargetResult = ExecutePayload(singleTargetPayload);
            if (singleTargetResult != null)
            {
                results.Add(singleTargetResult);
            }
        }

        switch (aOEPayload)
        {
            case AOEDamagePayload damagePayload:
                return new AOEDamageResult(damagePayload.Sender, damagePayload.Target, results.Cast<DamageResult>().ToList().AsReadOnly(), damagePayload.FractionOfDamageToSecondaryTargets, damagePayload.ParentResult, damagePayload, damagePayload.Originator);
            default:
                throw new ArgumentException("Unsupported AOEPayload type.");
        }
    }

	private void KillUnit(IReadOnlyUnit unit)
	{
		if (unit == null) return;

		if (!unit.InCombat) return;

        CombatTeam team = GetTeamWithUnit(unit);

        team.KillUnit(unit);

        if (team.PlayerControlled)
        {
            InsertUnitToInventory((Unit)unit); //TODO type safety
        } 
    }

    private void RetreatUnit(IReadOnlyUnit unit)
    {
        if (unit == null) return;

        if (!unit.InCombat) return;

        CombatTeam team = GetTeamWithUnit(unit);

        team.RetreatUnit(unit);

        if (team.PlayerControlled)
        {
            InsertUnitToInventory((Unit)unit);
        }
    }

    private void GiveEffectResultToTeams(ResultBase result)
	{
        Unit target = null;
        Unit sender = null;

        if (result.Target != null)
        {
            target = (Unit)result.Target; //TODO type safety
        }

        if (result.Sender != null)
        {
            sender = (Unit)result.Sender;
        }

		// Results reacted to in order: sender, sender's team (may include target), other team, target*
		// if sender is null, target's team is used first instead
		CombatTeam firstTeam = null;
        CombatTeam secondTeam = null;

        if (sender == null)
        {
            if (target == null)
            {
                firstTeam = GetPlayerTeam();
            }
            else
            {
                firstTeam = GetTeamWithUnit(target);
            }
        }
        else
        {
            firstTeam = GetTeamWithUnit(sender);
        }

		if (firstTeam == null) firstTeam = GetPlayerTeam();
        secondTeam = GetEnemyTeam(firstTeam);

        if (firstTeam == null || secondTeam == null) throw new ArgumentException("Invalid teams for effect result processing.");
		firstTeam.ReactToPayloadResult(result);
		secondTeam.ReactToPayloadResult(result);
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

public class RoundEndEventArgs : EventArgs
{

}

public class GetUnitsInInventoryEventArgs : EventArgs
{
	public List<IReadOnlyUnit> UnitsInInventory;
}