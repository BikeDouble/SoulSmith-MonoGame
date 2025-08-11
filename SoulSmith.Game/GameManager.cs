using Microsoft.Xna.Framework;
using System;
using System.Collections.ObjectModel;
using SoulSmith.UnitStats;
using SoulSmith.Object.Canvas;
using SoulSmith.Camp;
using SoulSmith.Combat;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using SoulSmith.MoveSelection;
using SoulSmith.Units;
using SoulSmith.Battle;
using SoulSmith.Drawing;

namespace SoulSmith.Game;
public partial class GameManager : CanvasObject
{

	public const string UIFONTNAME = "uIFont";

	//Children
	private CombatManager _combatManager;
	private CampManager _campManager;
	private UnitInventory _unitInventory;
	private GameHeaderUI _headerUI;

	public GameManager() 
	{
		Initialize();
    }

	private void Initialize()
	{
		InitializeHeaderUI();
		InitializeCombat();
		InitializeCamp();
		InitializeUnitInventory();
	}

	private void InitializeHeaderUI()
	{
		_headerUI = new GameHeaderUI();
		AddChild(_headerUI);
		_headerUI.UnitInventoryButtonPressedEventHandler += OnUnitInventoryButtonPressed;
		_headerUI.UnitListUIClickedOutsideEventHandler += OnUnitListUIClickedOutside;
		_headerUI.UnitListUIEntryPressedEventHandler += OnUnitListUIEntryPressed;
    }

	private void InitializeCamp()
	{
		_campManager = new CampManager();
		AddChild(_campManager);
	}

	private void InitializeCombat()
	{
		_combatManager = new CombatManager(new MoveSelector_PlayerInput(), new MoveSelector_Random());
		AddChild(_combatManager);

		_combatManager.OfferUnitToInventoryEventHandler += OnOfferUnitToInventory;
		_combatManager.RoundEndEventHandler += ProcessRoundEnd;
		_combatManager.GetUnitsInInventoryEventHandler += OnGetUnitsInInventoryInArgs;
		_combatManager.DeployUnitButtonPressedEventHandler += OnDeployUnitButtonPressed;
        _combatManager.BeginRound();
	}

	private void InitializeUnitInventory()
	{
		_unitInventory = new UnitInventory();
	}

	/// <summary>
	/// Returns the instantiated UnitSprite from the sprite name, or null
	/// </summary>
	/// <param name="spriteName"></param>
	/// <returns></returns>
	private static List<Effect> CopyEffectTemplate(ReadOnlyCollection<Effect> effects)
	{
		List<Effect> copiedEffects = new List<Effect>(effects.Count);

		foreach (Effect effect in effects)
		{
			copiedEffects.Add(effect);
		}

		return copiedEffects;
	}

    //Listens to combat manager and camp manager
    private void OnOfferUnitToInventory(object sender, OfferUnitToInventoryEventArgs e)
	{
		Unit unit = e.Unit;
		_unitInventory.AddUnit(unit);
	}

	//Listens to combat manager
	private void ProcessRoundEnd(object sender, RoundEndEventArgs e)
	{
		_combatManager.BeginRound();
		_campManager.OnRoundEnd();
	}

	private void OnGetUnitsInInventoryInArgs(object sender, GetUnitsInInventoryEventArgs e)
	{
		e.UnitsInInventory = _unitInventory.GetUnitsAsReadOnly();
	}

	private void OnUnitInventoryButtonPressed(object sender, UnitInventoryButtonPressedEventArgs e)
	{
		_headerUI.ShowUnitInventory(_unitInventory.GetUnitsAsReadOnly());
    }

	private void OnUnitListUIClickedOutside(object sender, ButtonPressedEventArgs e)
	{
		_headerUI.HideUnitInventory();
		_positionAwaitingUnit = null;
    }

	private void OnUnitListUIEntryPressed(object sender, UnitListUIEntryPressedEventArgs e)
	{
		if (_positionAwaitingUnit != null)
		{
			Unit unit = _unitInventory.GetMatchingUnit(e.Unit);

			if (unit != null)
			{
				_combatManager.DeployUnitAtPosition(_positionAwaitingUnit, unit);
				_unitInventory.RemoveUnit(unit);
                _positionAwaitingUnit = null;
				_headerUI.HideUnitInventory();
            }
		}
	}

	private IReadOnlyTeamPosition _positionAwaitingUnit;

    // Listens to combat
    private void OnDeployUnitButtonPressed(object sender, DeployUnitButtonPressedEventArgs e)
    {
		_positionAwaitingUnit = e.CallingPosition;

		if (_positionAwaitingUnit != null)
		{
			_headerUI.ShowUnitInventory(_unitInventory.GetUnitsAsReadOnly());
		}
    }
}

