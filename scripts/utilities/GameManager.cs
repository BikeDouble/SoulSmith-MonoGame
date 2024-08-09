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

	private AssetLoader _assetLoader;

	//private static AssetManager<Resource> _effectVisualizationAssetManager;

	public GameManager(ContentManager content)
	{
		Initialize(content);
	}

	private void Initialize(ContentManager content)
	{

		InitializeResources(content);
		InitializeCombat();
		InitializeCamp();
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
		_combatManager = new CombatManager(_assetLoader);
		AddChild(_combatManager);

		_combatManager.OfferUnitToInventoryEventHandler += OnOfferUnitToInventory;
		_combatManager.RoundEndEventHandler += ProcessRoundEnd;
		_combatManager.UnitInstantiationEventHandler += InstantiateUnitInArgs;
        _combatManager.BeginRound();
	}

    

    
	/// <summary>
	/// Instantiates unit from given template name, returns null if template not found
	/// </summary>
	/// <param name="templateName"></param>
	/// <returns></returns>
	private Unit InstantiateUnitFromTemplateName(string templateName)
	{
		UnitTemplate template = _assetLoader.GetUnitTemplate(templateName);
		
		if (template == null)
		{
			return null;
		}

		return InstantiateUnitFromTemplate(template);
	}

    /// <summary>
    /// Instantiates unit from given template, returns null if bad template
    /// </summary>
    /// <param name="templateName"></param>
    /// <returns></returns>
    private Unit InstantiateUnitFromTemplate(UnitTemplate template)
	{
		if (template == null) { return null; }

		StatsList stats = new StatsList(template.StatsList);
		List<Move> moves = InstantiateMovesetFromUnitTemplate(template);
		UnitSprite sprite = InstantiateSpriteFromSpriteName(template.SpriteName);
		UnitUI uI = InstantiateUnitUI();
		EmotionTag emotion = template.Emotion;
		string friendlyName = template.FriendlyName;
		int timeOnBoard = template.TimeOnBoard;
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = new();
        boundingZones.Add(BoundingZoneType.EffectSender, sprite);
        boundingZones.Add(BoundingZoneType.EffectReceiver, sprite);

        Unit unit = new Unit(stats, moves.AsReadOnly(), sprite, boundingZones, uI, emotion, friendlyName, timeOnBoard);
		return unit;
	}

	private UnitUI InstantiateUnitUI()
	{
		DrawableResource_Polygon moveButton = new DrawableResource_Polygon(_assetLoader.GetPolygon("MoveButton"));
		DrawableResource_Polygon targetButtonIdle = new DrawableResource_Polygon(_assetLoader.GetPolygon("TargetButtonIdle"));
		DrawableResource_Polygon targetButtonHovered = new DrawableResource_Polygon(_assetLoader.GetPolygon("TargetButtonHovered"));
        SpriteFont font = _assetLoader.GetFont(UIFONTNAME);
		
		UnitUI unitUI = new UnitUI(font, moveButton, targetButtonIdle, targetButtonHovered);

		return unitUI;
	}

	/// <summary>
	/// Instantiates unit from template name and places it in the wrapper. Used for child classes to call for unit instantiation.
	/// </summary>
	/// <param name="unitTemplateName"></param>
	/// <param name="wrapper"></param>
	private void InstantiateUnitInArgs(object sender, UnitInstantiationEventArgs e)
	{
		if (e == null) { return; }

		Unit unit = InstantiateUnitFromTemplateName(e.UnitTemplateName);

		e.Unit = unit;
	}

	/// <summary>
	/// Returns the instantiated UnitSprite from the sprite name, or null
	/// </summary>
	/// <param name="spriteName"></param>
	/// <returns></returns>
	private UnitSprite InstantiateSpriteFromSpriteName(string spriteName)
	{
		UnitSprite sprite = new UnitSprite((CanvasItem_TransformationRules)_assetLoader.GetSprite(spriteName));

		return sprite;
	}

	private List<Move> InstantiateMovesetFromUnitTemplate(UnitTemplate template) 
	{
        if (template == null)
		{
			return new List<Move>();
		}
		
		if (template.MoveSetString.Count > template.MaxMoveCount)
		{
			//TODO make randomizable move set
            return InstantiateMoveSetFromMoveSetString(template.MoveSetString, template.MaxMoveCount);
        } 
		else
		{
			return InstantiateMoveSetFromMoveSetString(template.MoveSetString);
		}

    }

	private List<Move> InstantiateMoveSetFromMoveSetString(ReadOnlyCollection<string> moveSetString, int maxMoveCount = 3)
	{
		List<Move> moves = new List<Move>();
		string moveString;
		int max = Math.Min(maxMoveCount, moveSetString.Count);

        for (int i = 0; i < max; i++)
        {
			moveString = moveSetString[i];
			Move move = InstantiateMoveFromMoveTemplateName(moveString);

			if (move != null)
				moves.Add(move);
        }

		return moves;
    }

	private Move InstantiateMoveFromMoveTemplateName(string name)
	{
		MoveTemplate moveTemplate = _assetLoader.GetMoveTemplate(name);
		return InstantiateMoveFromMoveTemplate(moveTemplate);
    }

	private Move InstantiateMoveFromMoveTemplate(MoveTemplate template)
	{
		if (template == null)
			return null;

		List<Effect> effects = _assetLoader.LoadMultipleEffectsFromTemplates(template.Effects);

		Move move = new Move(template, _assetLoader.GetEmotion(template.EmotionTag), effects);

		return move;
	}

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

    //Listens to combat manager
    private void OnOfferUnitToInventory(object sender, OfferUnitToInventoryEventArgs e)
	{
		Unit unit = e.Unit;
		_campManager.AddUnitToInventory(unit);
	}

	//Listens to combat manager
	private void ProcessRoundEnd(object sender, RoundEndEventArgs e)
	{
		_combatManager.BeginRound();
		_campManager.OnRoundEnd();
	}

    public override void GetAssetLoaderInternal(object sender, GetAssetLoaderEventArgs e)
    {
		e.AssetLoader = _assetLoader;
    }
}


