using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text.Json;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using SoulSmithEmotions;
using SoulSmithStats;
using System.Diagnostics;
using SoulSmithMoves;
using SoulSmithModifiers;
using SoulSmithDeserialization;
using System.Linq;
using KaimiraGames;
using SoulSmithObjects;
using SoulSmithJsonConversion;


public class AssetLoader : SoulSmithObject
{
    //Im bad at files
    public const string FILEPREFIX = "../../../";

    //File paths - preface all with ../../../ im sure this will result in no problems for me in the future
    private const string POLYGONASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/polygonAssetsList.json";
    private const string SPRITEASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/spriteAssetsList.json";
    private const string UNITTEMPLATEASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/unitTemplateAssetsList.json";
    private const string SPRITESHELLASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/spriteShellAssetsList.json";
    private const string WEIGHTEDLISTASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/weightedListAssetsList.json";
    private const string MASTERENEMYSPAWNLISTJSONFILEPATH = "json/masterEnemySpawnList.json";

    private static Dictionary<string, MoveTemplate> _moveTemplateLibrary;
    private static Dictionary<string, SpriteFont> _fontLibrary;
    private static Dictionary<string, EffectVisualizationTemplate> _effectVisualizationTemplateLibrary;
    private static Dictionary<string, ModifierTemplate> _modifierTemplateLibrary;
    private static Dictionary<EmotionTag, Emotion> _emotionLibrary;

    private static AssetManager<ColoredPolygon> _polygonAssetManager;
    private static AssetManager<CanvasItem> _spriteAssetManager;
    private static AssetManager<CanvasItem_TransformationRules> _spriteShellAssetManager;
    private static AssetManager<UnitTemplate> _unitTemplateAssetManager;
    private static AssetManager<SoulSmithWeightedList<string>> _weightedListAssetManager;

    private readonly ContentManager _content;

    public AssetLoader(ContentManager content)
    {
        _content = content;

        InitializeAssetManagers(content);
        InitializeLibraries(content);
    }

    private void InitializeLibraries(ContentManager content)
    {
        _effectVisualizationTemplateLibrary = EffectVisualizationTemplateLibrary.CreateDict();

        _modifierTemplateLibrary = ModifierTemplateLibrary.CreateDict();

        _emotionLibrary = EmotionLibrary.CreateDict();

        _moveTemplateLibrary = MoveTemplateLibrary.CreateDict();

        _fontLibrary = new();
        _fontLibrary.Add("uIFont", content.Load<SpriteFont>("fonts/uiFont"));
    }

    private void InitializeAssetManagers(ContentManager content)
    {
        _polygonAssetManager = new AssetManager<ColoredPolygon>(POLYGONASSETJSONFILEPATH, LoadPolygon);
        _polygonAssetManager.PreloadAll();

        _spriteShellAssetManager = new AssetManager<CanvasItem_TransformationRules>(SPRITESHELLASSETJSONFILEPATH, LoadSpriteShell);
        _spriteShellAssetManager.PreloadAll();

        _spriteAssetManager = new AssetManager<CanvasItem>(SPRITEASSETJSONFILEPATH, LoadSprite);
        _spriteAssetManager.PreloadAssetGroup("forms");

        _unitTemplateAssetManager = new AssetManager<UnitTemplate>(UNITTEMPLATEASSETJSONFILEPATH, LoadUnitTemplate);
        _unitTemplateAssetManager.PreloadAll();

        _weightedListAssetManager = new AssetManager<SoulSmithWeightedList<string>>(WEIGHTEDLISTASSETJSONFILEPATH, LoadWeightedList);
        _weightedListAssetManager.PreloadAll();

        //_effectVisualizationAssetManager = new AssetManager<PackedScene>(EFFECTVISUALIZATIONJSONFILEPATH);
    }

    public static ModifierTemplate GetModifierTemplate(string name)
    {
        return _modifierTemplateLibrary.GetValueOrDefault(name);
    }

    public static MoveTemplate GetMoveTemplate(string assetName)
    {
        return _moveTemplateLibrary.GetValueOrDefault(assetName);
    }

    public static SpriteFont GetFont(string assetName)
    {
        return _fontLibrary.GetValueOrDefault(assetName);
    }

    public static EffectVisualizationTemplate GetEffectVisualizationTemplate(string assetName)
    {
        return _effectVisualizationTemplateLibrary.GetValueOrDefault(assetName);
    }

    public static Emotion GetEmotion(EmotionTag tag)
    {
        return _emotionLibrary.GetValueOrDefault(tag);
    }

    public static TrackedResource<ColoredPolygon> GetPolygon(string assetName)
    {
        return _polygonAssetManager.GetAsset(assetName);
    }

    public static TrackedResource<CanvasItem> GetSprite(string assetName)
    {
        return _spriteAssetManager.GetAsset(assetName);
    }

    public static SoulSmithWeightedList<string> GetWeightedList(string name)
    {
        return _weightedListAssetManager.GetAsset(name);
    }

    public static TrackedResource<UnitTemplate> GetUnitTemplate(string assetName)
    {
        return _unitTemplateAssetManager.GetAsset(assetName);
    }

    public static TrackedResource<CanvasItem_TransformationRules> GetSpriteShell(string assetName)
    {
        return _spriteShellAssetManager.GetAsset(assetName);
    }

    /// <summary>
	/// Loads visualization with stored visualization name for effect and all child effects
	/// </summary>
	/// <param name="template"></param>
	public static Effect LoadEffectFromTemplate(EffectTemplate template)
    {
        if (template == null) return null;

        EffectVisualizationTemplate visTemplate = null;

        if ((template.VisualizationName != null)
            && (template.VisualizationName != "")
            && (template.VisualizationName != "none"))
            visTemplate = GetEffectVisualizationTemplate(template.VisualizationName);

        List<Effect> childEffects = LoadMultipleEffectsFromTemplates(template.ChildEffects);

        Effect effect = new Effect(template.GenerateEffectRequest,
            template.TargetingStyle,
            childEffects,
            visTemplate,
            template.VisualizationDelay,
            template.RequiresPriority,
            template.SwapSenderAndTarget);

        return effect;
    }

    public static List<Effect> LoadMultipleEffectsFromTemplates(IEnumerable<EffectTemplate> effectTemplates)
    {
        if ((effectTemplates == null) || (effectTemplates.Count() == 0))
            return null;

        List<Effect> effects = new List<Effect>(effectTemplates.Count());

        foreach (EffectTemplate template in effectTemplates)
        {
            effects.Add(LoadEffectFromTemplate(template));
        }

        return effects;
    }

    public static Func<string, SoulSmithWeightedList<string>> LoadWeightedList = (assetPath)
        => LoadWeightedListInternal(assetPath);

    private static SoulSmithWeightedList<string> LoadWeightedListInternal(string assetPath)
    {
        string jsonString = ReadJsonToString(assetPath);
        var serializationOptionsWithConverter = new JsonSerializerOptions();
        serializationOptionsWithConverter.Converters.Add(new SoulSmithWeightedListJsonConverter<string>());
        return JsonSerializer.Deserialize<SoulSmithWeightedList<string>>(jsonString, serializationOptionsWithConverter);
    }

    public static List<(SoulSmithWeightedList<string> List, string Name, int ActivationRound)> LoadMasterEnemySpawnList()
    {
        DeserializedMasterEnemySpawnList dMasterList = ReadJson<DeserializedMasterEnemySpawnList>(MASTERENEMYSPAWNLISTJSONFILEPATH);

        List<(SoulSmithWeightedList<string> List, string Name, int ActivationRound)> masterList = new();
        
        foreach (var dList in dMasterList.Lists)
        {
            (SoulSmithWeightedList<string> List, string Name, int ActivationRound) listItem = new();
            listItem.List = GetWeightedList(dList.Name);
            listItem.Name = dList.Name;
            listItem.ActivationRound = dList.ActivationRound;
            masterList.Add(listItem);
        }

        return masterList;
    }

    public static Func<string, UnitTemplate> LoadUnitTemplate = (string assetPath)
            => ReadJson<UnitTemplate>(assetPath);

    public static Func<string, ColoredPolygon> LoadPolygon = (string assetPath)
        => ReadJson<ColoredPolygon>(assetPath);    

    public static Func<string, CanvasItem_TransformationRules> LoadSpriteShell = (path) =>
        LoadSpriteShellInternal(path);

    private static CanvasItem_TransformationRules LoadSpriteShellInternal(string assetPath)
    {
        DeserializedSpriteShell shell = ReadJson<DeserializedSpriteShell>(assetPath);

        List<CanvasTransformationRule> rules = new List<CanvasTransformationRule>();

        foreach (DeserializedCanvasTransformationRule dRule in shell.MovementRules)
        {
            CanvasTransformationRule rule = InstantiateCanvasTransformationRule(dRule, null);
            rules.Add(rule);
        }

        if (rules.Count > 0)
            return new CanvasItem_TransformationRules(rules);

        return null;
    }

    public static Func<string, CanvasItem> LoadSprite = (assetPath) => LoadSpriteInternal(assetPath);

    private static CanvasItem LoadSpriteInternal(string assetPath)
    {
        DeserializedSprite deserializedSprite = ReadJson<DeserializedSprite>(assetPath);
        DeserializedSpritePart[] deserializedParts = deserializedSprite.Parts;

        List<CanvasTransformationRule> rules = new List<CanvasTransformationRule>();
        List<SoulSmithObject> unrulyChildren = new List<SoulSmithObject>();
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = new Dictionary<BoundingZoneType, CanvasItem>();

        if (deserializedSprite.BoundingZones != null)
        {
            boundingZones = InstantiateBoundingZones(deserializedSprite.BoundingZones);
        }
        else
        {
            boundingZones = new Dictionary<BoundingZoneType, CanvasItem>();
        }

        foreach (DeserializedSpritePart part in deserializedParts) 
        {
            CanvasItem child = InstantiateCanvasItem(part.ResourceName, part.ResourceType, part.PositionArgs, part.BoundingZones);

            if (part.MovementRules == null)
            {
                unrulyChildren.Add(child);
            }
            else
            {
                foreach (DeserializedCanvasTransformationRule dRule in part.MovementRules)
                {
                    CanvasTransformationRule rule = InstantiateCanvasTransformationRule(dRule, child);

                    if (rule != null)
                    {
                        rules.Add(rule);
                    }
                    else
                    {
                        unrulyChildren.Add(child);
                    }
                }
            }

            if (child.BoundingZones != null)
            {
                foreach (KeyValuePair<BoundingZoneType, CanvasItem> item in  child.BoundingZones)
                {
                    boundingZones.TryAdd(item.Key, child);
                }
            }
        }

        CanvasItem sprite;

        if (rules.Count == 0)
        {
            sprite = new CanvasItem(new CanvasPosition(deserializedSprite.PositionArgs), null, boundingZones, unrulyChildren);
        }
        else
        {
            sprite = new CanvasItem_TransformationRules(rules, unrulyChildren, boundingZones, new CanvasPosition(deserializedSprite.PositionArgs));
        }

        if (deserializedSprite.ShellName != null)
        {
            TrackedResource<CanvasItem_TransformationRules> trackedShell = GetSpriteShell(deserializedSprite.ShellName);
            sprite = (CanvasItem)trackedShell.Resource.DeepClone(sprite);
        }

        return sprite;
    }

    private static CanvasItem InstantiateCanvasItem(string resourceName, string resourceType, float[] positionArgs, DeserializedBoundingZone[] deserializedboundingZones = null)
    {
        resourceType = resourceType.ToLower();
        Dictionary<BoundingZoneType, CanvasItem> boundingZones;

        switch (resourceType)
        {
            case ("polygon"):
                TrackedResource<ColoredPolygon> polygon = GetPolygon(resourceName);

                if (polygon == null)
                    return null;

                boundingZones = InstantiateBoundingZones(deserializedboundingZones);
                return new CanvasItem(null, new DrawableResource_Polygon(polygon), boundingZones);

            /*case ("texture"):
                TrackedResource<Texture2D> texture = GetTextureAsset(resourceName);

                if (texture == null)
                    return null;

                return new CanvasItem(texture);*/

            case ("sprite"):
                TrackedResource<CanvasItem> sprite = GetSprite(resourceName);

                if (sprite == null) 
                    return null;

                CanvasItem newSprite = (CanvasItem)sprite.Resource.DeepClone();
                newSprite.Set(new CanvasPosition(positionArgs));

                return newSprite;

            case ("none"):
                return null;

            default:
                return null;
        }
    }

    private const bool SHOWBOUNDINGZONEOUTLINE = false;

    private static Dictionary<BoundingZoneType, CanvasItem> InstantiateBoundingZones(DeserializedBoundingZone[] deserializedBoundingZones)
    {
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = null;

        if (deserializedBoundingZones != null)
        {
            boundingZones = new();
            foreach (DeserializedBoundingZone deserializedZone in deserializedBoundingZones)
            {
                (BoundingZoneType[], CanvasItem) zoneTypeList = InstantiateBoundingZoneTypeTuple(deserializedZone);

                if ((zoneTypeList.Item1 != null) && (zoneTypeList.Item2 != null))
                {
                    foreach (BoundingZoneType zoneType in zoneTypeList.Item1)
                    {
                        if (zoneType != BoundingZoneType.None)
                            boundingZones.Add(zoneType, zoneTypeList.Item2);
                    }
                }
            }

            if (boundingZones.Count < 1)
                boundingZones = null;

        }

        return boundingZones;
    }

    private static (BoundingZoneType[], BoundingZone) InstantiateBoundingZoneTypeTuple(DeserializedBoundingZone deserializedZone)
    {
        string zoneShape = deserializedZone.Shape.ToLower();
        BoundingZone zone;
        CanvasPosition position = new CanvasPosition(deserializedZone.PositionArgs);

        switch (zoneShape)
        {
            case ("ellipse"):
                zone = new BoundingZone_Ellipse(deserializedZone.ZoneArgs[0], SHOWBOUNDINGZONEOUTLINE, position);
                break;

            default:
                return (null, null);
        }

        BoundingZoneType[] types = ConvertBoundingZoneTypesStringToEnum(deserializedZone.Types);

        return (types, zone);
    }

    private static BoundingZoneType[] ConvertBoundingZoneTypesStringToEnum(string[] stringTypes)
    {
        BoundingZoneType[] types = new BoundingZoneType[stringTypes.Length];

        if (types.Length < 1)
            return null;

        for (int i = 0; i < types.Length; i++)
        {
            string typeString = stringTypes[i].ToLower();
            switch (typeString)
            {
                case ("esender"):
                    types[i] = BoundingZoneType.EffectSender; break;
                case ("ereceiver"):
                    types[i] = BoundingZoneType.EffectReceiver; break;
                default:
                    types[i] = BoundingZoneType.None; break;
            }
        }

        return types;
    }

    private static CanvasTransformationRule InstantiateCanvasTransformationRule(DeserializedCanvasTransformationRule dRule, ITransformable transformedItem)
    {
        string type = dRule.TransformationType.ToLower();
        Func<float, ReadOnlyCollection<float>, float> velocityFunc = GetVelocityFunc(dRule.VelocityFunction);
        Action<TransformaionRuleDelegateArgs, ITransformable, double, float> transformDelegate;
        TransformaionRuleDelegateArgs transformationDelegateArgs;
        float duration = GetTrueTransformationRuleDuration(dRule.TransformationDuration);
        bool peakIsTarget = dRule.PeakIsTarget;
        float cycleDuration = GetTrueTransformationRuleDuration(dRule.CycleDuration);

        switch (type)
        {
            case ("rotation"):
                if (dRule.Transformation.Length < 3)
                    return null;

                Vector2 origin = new Vector2(dRule.Transformation[0], dRule.Transformation[1]);

                float rotation = dRule.Transformation[2];

                float peakRotationDiff = 0;
                
                if (dRule.PeakTransformation != null)
                    peakRotationDiff = dRule.PeakTransformation[2] - rotation;

                transformationDelegateArgs = new(
                    new List<float> { (float)(rotation * Math.PI / 180) }, 
                    new List<float> { (float)(peakRotationDiff * Math.PI / 180) }, origin);

                transformDelegate = CanvasTransformationRule.RotationDelegate;

                break;

            case ("translation"):
                if (dRule.Transformation.Length < 2)
                    return null;

                List<float> translation = new List<float>{ dRule.Transformation[0], dRule.Transformation[1] };

                List<float> peakTranslationDiff = null;

                if (dRule.PeakTransformation != null)
                    peakTranslationDiff = new List<float> {
                        dRule.PeakTransformation[0] - dRule.Transformation[0],
                        dRule.PeakTransformation[1] - dRule.Transformation[1]};

                transformationDelegateArgs = new(translation, peakTranslationDiff, Vector2.Zero);

                transformDelegate = CanvasTransformationRule.TranslationDelegate;

                break;

            case ("scaleadditive"):
                if (dRule.Transformation.Length < 2)
                    return null;

                List<float> scale = new List<float> { dRule.Transformation[0], dRule.Transformation[1] };

                List<float> peakScaleDiff = null;

                if (dRule.PeakTransformation != null)
                    peakScaleDiff = new List<float> {
                        dRule.PeakTransformation[0] - dRule.Transformation[0],
                        dRule.PeakTransformation[1] - dRule.Transformation[1]};

                transformationDelegateArgs = new(scale, peakScaleDiff, Vector2.Zero);

                transformDelegate = CanvasTransformationRule.ScaleAdditiveDelegate;

                break;

            case ("vertexTranslation"):
                if (dRule.Transformation.Length < 2)
                    return null;

                if (dRule.VerticeIndices.Length == 0)
                    return null;

                translation = new List<float> { dRule.Transformation[0], dRule.Transformation[1] };

                peakTranslationDiff = null;

                if (dRule.PeakTransformation != null)
                    peakTranslationDiff = new List<float> {
                        dRule.PeakTransformation[0] - dRule.Transformation[0],
                        dRule.PeakTransformation[1] - dRule.Transformation[1]};

                //TODO
                transformationDelegateArgs = new(translation, peakTranslationDiff, Vector2.Zero);

                transformDelegate = CanvasTransformationRule.VertexTranslationDelegate; break;

            default: 
                return null;
        }

        return new CanvasTransformationRule(
            transformedItem,
            dRule.ActiveStates,
            transformationDelegateArgs,
            transformDelegate,
            duration,
            velocityFunc,
            dRule.PeakIsTarget,
            cycleDuration,
            dRule.Tag);
    }

    public static Unit InstantiateUnit(string templateName)
    {
        UnitTemplate template = AssetLoader.GetUnitTemplate(templateName);

        if (template == null)
        {
            return null;
        }

        return InstantiateUnit(template);
    }

    public static Unit InstantiateUnit(UnitTemplate template)
    {
        if (template == null) { return null; }

        StatsList stats = new StatsList(template.StatsList);
        List<Move> moves = InstantiateMoveSet(template);
        UnitSprite sprite = InstantiateUnitSprite(template.SpriteName);
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

    private static UnitSprite InstantiateUnitSprite(string spriteName)
    {
        float animationDesync = (float)Rand.RandDoubleAroundOne(UnitSprite.ANIMATIONDESYNCFACTORRADIUS);
        UnitSprite sprite = new UnitSprite((CanvasItem)AssetLoader.GetSprite(spriteName), animationDesync);

        return sprite;
    }

    private static UnitUI InstantiateUnitUI()
    {
        DrawableResource_Polygon moveButton = new DrawableResource_Polygon(AssetLoader.GetPolygon("MoveButton"));
        DrawableResource_Polygon targetButtonIdle = new DrawableResource_Polygon(AssetLoader.GetPolygon("TargetButtonIdle"));
        DrawableResource_Polygon targetButtonHovered = new DrawableResource_Polygon(AssetLoader.GetPolygon("TargetButtonHovered"));
        SpriteFont font = AssetLoader.GetFont(GameManager.UIFONTNAME);

        UnitUI unitUI = new UnitUI(font, moveButton, targetButtonIdle, targetButtonHovered);

        return unitUI;
    }

    private static List<Move> InstantiateMoveSet(UnitTemplate template)
    {
        if (template == null)
        {
            return new List<Move>();
        }

        if (template.MoveSetString.Count > template.MaxMoveCount)
        {
            //TODO make randomizable move set
            return InstantiateMoveSet(template.MoveSetString, template.MaxMoveCount);
        }
        else
        {
            return InstantiateMoveSet(template.MoveSetString);
        }
    }

    private static List<Move> InstantiateMoveSet(ReadOnlyCollection<string> moveSetString, int maxMoveCount = 3)
    {
        List<Move> moves = new List<Move>();
        string moveString;
        int max = Math.Min(maxMoveCount, moveSetString.Count);

        for (int i = 0; i < max; i++)
        {
            moveString = moveSetString[i];
            Move move = InstantiateMove(moveString);

            if (move != null)
                moves.Add(move);
        }

        return moves;
    }

    private static Move InstantiateMove(string name)
    {
        MoveTemplate moveTemplate = AssetLoader.GetMoveTemplate(name);
        return InstantiateMove(moveTemplate);
    }

    private static Move InstantiateMove(MoveTemplate template)
    {
        if (template == null)
            return null;

        List<Effect> effects = AssetLoader.LoadMultipleEffectsFromTemplates(template.Effects);

        Move move = new Move(template, AssetLoader.GetEmotion(template.EmotionTag), effects);

        return move;
    }

    private static float GetTrueTransformationRuleDuration(float duration)
    {
        switch (duration)
        {
            case -7:
                duration = (float) UnitSprite.ATTACKANIMATIONDURATION; break;
            case -8:
                duration = (float) UnitSprite.HURTANIMATIONDURATION; break;
            case -9:
                duration = (float) UnitSprite.DEATHANIMATIONDURATION; break;
        }

        return duration;
    }

    private static Func<float, ReadOnlyCollection<float>, float> GetVelocityFunc(string name)
    {
        if (name == null) return null;

        switch (name.ToLower())
        {
            case "smoothbell":
                return CanvasTransformationRule.SmoothBell;
            case "slope":
                return CanvasTransformationRule.Slope;
            case "slopepartwaypeak":
                return CanvasTransformationRule.SlopePartwayPeak;
            case "sin":
                return CanvasTransformationRule.Sin;
            case "formfloat":
                return CanvasTransformationRule.FormFloat;
            default:
                return null;
        }
    }

    public static string ReadJsonToString(string jsonPath)
    {
        jsonPath = FILEPREFIX + jsonPath;

        string jsonString = File.ReadAllText(jsonPath);
        return jsonString;
    }

    public static T ReadJson<T>(string jsonPath)
    {
        string jsonString = ReadJsonToString(jsonPath);

        return JsonSerializer.Deserialize<T>(jsonString);
    }

    /// <summary>
    /// Requires AssetLoader instance
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    private Texture2D LoadTexture2DInternal(string assetPath)
    {
        return _content.Load<Texture2D>(assetPath);
    }
}



