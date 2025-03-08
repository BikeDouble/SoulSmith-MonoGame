using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.ObjectModel;
using SoulSmithStats;
using SoulSmithMoves;
using SoulSmithEmotions;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace SoulSmithObjects;
public partial class GameManager : CanvasItem
{

	public const string UIFONTNAME = "uIFont";

	//Children
	private CombatManager _combatManager;
	private CampManager _campManager;
	private UnitInventory _unitInventory;
	private AssetLoader _assetLoader;

	public GameManager(ContentManager content)
	{
		Initialize(content);
	}

	private void Initialize(ContentManager content)
	{

		InitializeResources(content);
		InitializeCombat();
		InitializeCamp();
		InitializeUnitInventory();
	}

	private void InitializeResources(ContentManager content)
	{
		_assetLoader = new(content);
	}

	private void InitializeCamp()
	{
		_campManager = new CampManager();
		AddChild(_campManager);
	}

	private void InitializeCombat()
	{
		_combatManager = new CombatManager();
		AddChild(_combatManager);

		_combatManager.OfferUnitToInventoryEventHandler += OnOfferUnitToInventory;
		_combatManager.RoundEndEventHandler += ProcessRoundEnd;
		_combatManager.UnitInstantiationEventHandler += InstantiateUnitInArgs;
        _combatManager.BeginRound();
	}

	private void InitializeUnitInventory()
	{
		_unitInventory = new UnitInventory();
	}
	

	/// <summary>
	/// Instantiates unit from template name and places it in the wrapper. Used for child classes to call for unit instantiation.
	/// </summary>
	/// <param name="unitTemplateName"></param>
	/// <param name="wrapper"></param>
	private void InstantiateUnitInArgs(object sender, UnitInstantiationEventArgs e)
	{
		if (e == null) { return; }

		Unit unit = AssetLoader.InstantiateUnit(e.UnitTemplateName);

		e.Unit = unit;
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

	/// <summary>
	/// Used statically to instantiate effect with no visualization and no child effects
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public static Effect InstantiateNakedEffect(EffectTemplate template)
	{
        Effect effect = new Effect(template.GenerateEffectRequest,
            template.TargetingStyle,
            null,
            null,
            template.VisualizationDelay,
            template.RequiresPriority,
            template.SwapSenderAndTarget);

        return effect;
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
}

