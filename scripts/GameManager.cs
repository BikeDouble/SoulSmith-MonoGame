using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.ObjectModel;
using SoulSmithStats;
using SoulSmithMoves;
using SoulSmithEmotions;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

public partial class GameManager : CanvasItem
{
	//File paths - preface all with ../../../ im sure this will result in no problems for me in the future
	private const string POLYGONASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/polygonAssetsList.json";
	private const string SPRITEASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/spriteAssetsList.json";
	private const string UNITTEMPLATEASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/unitTemplateAssetsList.json";
	private const string EFFECTVISUALIZATIONJSONFILEPATH = AssetLoader.FILEPREFIX + "json/effectVisualizationAssetsList.json";

	//Children
	private CombatManager _combatManager;
	private CampManager _campManager;

	private static Dictionary<string, MoveTemplate> _moveTemplateLibrary;
	private static Dictionary<string, SpriteFont> _fontLibrary;
	private static Dictionary<EmotionTag, Emotion> _emotionLibrary;

	private static AssetLoader _assetLoader;
	private static AssetManager<ColoredPolygon> _polygonAssetManager;
	private static AssetManager<CanvasItem_TransformationRules> _spriteAssetManager;
	private static AssetManager<UnitTemplate> _unitTemplateAssetManager;
	//private static AssetManager<Resource> _effectVisualizationAssetManager;

	private int _roundCount = 1;

	public GameManager(ContentManager content)
	{
		Initialize(content);
	}

	private void Initialize(ContentManager content)
	{

		InitializeAssetManagers(content);
		InitializeLibraries(content);
		InitializeCombat();
		InitializeCamp();
	}

	private void InitializeLibraries(ContentManager content)
	{
		_moveTemplateLibrary = MoveTemplateLibrary.CreateDict();

		_emotionLibrary = EmotionLibrary.CreateDict();

		_fontLibrary = new();
		_fontLibrary.Add("uIFont", content.Load<SpriteFont>("fonts/uiFont"));
	}

	private void InitializeAssetManagers(ContentManager content)
	{
		_polygonAssetManager = new AssetManager<ColoredPolygon> (POLYGONASSETJSONFILEPATH, AssetLoader.LoadPolygon);
		_polygonAssetManager.PreloadAll();

		_assetLoader = new AssetLoader(content, this);

		_spriteAssetManager = new AssetManager<CanvasItem_TransformationRules>(SPRITEASSETJSONFILEPATH, _assetLoader.LoadSprite);
		_spriteAssetManager.PreloadAssetGroup("forms");

		_unitTemplateAssetManager = new AssetManager<UnitTemplate>(UNITTEMPLATEASSETJSONFILEPATH, _assetLoader.LoadUnitTemplate);
		_unitTemplateAssetManager.PreloadAll();
		//_effectVisualizationAssetManager = new AssetManager<PackedScene>(EFFECTVISUALIZATIONJSONFILEPATH);
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

    public TrackedResource<ColoredPolygon> GetPolygonAsset(string assetName)
    {
		return _polygonAssetManager.GetAsset(assetName);
    }

	public TrackedResource<CanvasItem_TransformationRules> GetSpriteAsset(string assetName)
	{
		return _spriteAssetManager.GetAsset(assetName);
	}

    
	/// <summary>
	/// Instantiates unit from given template name, returns null if template not found
	/// </summary>
	/// <param name="templateName"></param>
	/// <returns></returns>
	private Unit InstantiateUnitFromTemplateName(string templateName)
	{
		UnitTemplate template = _unitTemplateAssetManager.GetAsset(templateName);
		
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

		Unit unit = new Unit(stats, moves.AsReadOnly(), sprite, uI, emotion, friendlyName, timeOnBoard);
		return unit;
	}

	private UnitUI InstantiateUnitUI()
	{
		DrawableResource_Polygon moveButton = new DrawableResource_Polygon(GetPolygonAsset("MoveButton"));
		DrawableResource_Polygon targetButtonIdle = new DrawableResource_Polygon(GetPolygonAsset("TargetButtonIdle"));
		SpriteFont font = _fontLibrary["uIFont"];
		
		UnitUI unitUI = new UnitUI(font, moveButton, targetButtonIdle);

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
	private static UnitSprite InstantiateSpriteFromSpriteName(string spriteName)
	{
		UnitSprite sprite = new UnitSprite(_spriteAssetManager.GetAsset(spriteName));

		return sprite;
	}

	private static List<Move> InstantiateMovesetFromUnitTemplate(UnitTemplate template) 
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

	private static List<Move> InstantiateMoveSetFromMoveSetString(ReadOnlyCollection<string> moveSetString, int maxMoveCount = 3)
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

	private static Move InstantiateMoveFromMoveTemplateName(string name)
	{
		MoveTemplate moveTemplate = _moveTemplateLibrary[name];
		return InstantiateMoveFromMoveTemplate(moveTemplate);
    }

	private static Move InstantiateMoveFromMoveTemplate(MoveTemplate template)
	{
		if (template == null)
			return null;

		List<Effect> effects = InstantiateEffects(template.Effects);

		Move move = new Move(template, _emotionLibrary[template.EmotionTag], effects);

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

	private static List<Effect> InstantiateEffects(IEnumerable<EffectTemplate> effectTemplates)
	{
		if ((effectTemplates == null) || (effectTemplates.Count() == 0))
			return null;

		List<Effect> effects = new List<Effect>(effectTemplates.Count());

		foreach (EffectTemplate template in effectTemplates)
		{
			effects.Add(InstantiateEffect(template));
		}

		return effects;
	}

	/// <summary>
	/// Loads visualization with stored visualization name for effect and all child effects
	/// </summary>
	/// <param name="template"></param>
	public static Effect InstantiateEffect(EffectTemplate template)
	{
		//TrackedResource<CanvasItem_TransformationRules> sprite = _spriteAssetManager.GetAsset(template.VisualizationName);

		List<Effect> childEffects = InstantiateEffects(template.ChildEffects);

		Effect effect = new Effect(template.GenerateEffectRequest,
			template.TargetingStyle,
            childEffects,
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
		_roundCount++;
		//_combatManager.UpdateRoundCount(_roundCount);
	}
}
