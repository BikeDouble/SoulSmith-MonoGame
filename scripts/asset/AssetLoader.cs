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
using System.Linq;


public class AssetLoader : SoulSmithObject, IAssetLoadOnly
{
    //Im bad at files
    public const string FILEPREFIX = "../../../";

    //File paths - preface all with ../../../ im sure this will result in no problems for me in the future
    private const string POLYGONASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/polygonAssetsList.json";
    private const string SPRITEASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/spriteAssetsList.json";
    private const string UNITTEMPLATEASSETJSONFILEPATH = AssetLoader.FILEPREFIX + "json/unitTemplateAssetsList.json";
    private const string EFFECTVISUALIZATIONJSONFILEPATH = AssetLoader.FILEPREFIX + "json/effectVisualizationAssetsList.json";

    private static Dictionary<string, MoveTemplate> _moveTemplateLibrary;
    private static Dictionary<string, SpriteFont> _fontLibrary;
    private static Dictionary<string, EffectVisualizationTemplate> _effectVisualizationTemplateLibrary;
    private static Dictionary<string, ModifierTemplate> _modifierTemplateLibrary;
    private static Dictionary<EmotionTag, Emotion> _emotionLibrary;

    private static AssetManager<ColoredPolygon> _polygonAssetManager;
    private static AssetManager<CanvasItem> _spriteAssetManager;
    private static AssetManager<UnitTemplate> _unitTemplateAssetManager;

    private readonly ContentManager _content;

    public AssetLoader(ContentManager content)
    {
        _content = content;

        LoadSprite = (string assetPath)
            => LoadSpriteInternal(assetPath);

        LoadUnitTemplate = (string assetPath)
            => LoadUnitTemplateInternal(assetPath);

        InitializeAssetManagers(content);
        InitializeLibraries(content);
    }

    private void InitializeLibraries(ContentManager content)
    {
        _effectVisualizationTemplateLibrary = EffectVisualizationTemplateLibrary.CreateDict(this);

        _modifierTemplateLibrary = ModifierTemplateLibrary.CreateDict(this);

        _emotionLibrary = EmotionLibrary.CreateDict(this);

        _moveTemplateLibrary = MoveTemplateLibrary.CreateDict(this);

        _fontLibrary = new();
        _fontLibrary.Add("uIFont", content.Load<SpriteFont>("fonts/uiFont"));
    }

    private void InitializeAssetManagers(ContentManager content)
    {
        _polygonAssetManager = new AssetManager<ColoredPolygon>(POLYGONASSETJSONFILEPATH, LoadPolygon);
        _polygonAssetManager.PreloadAll();

        _spriteAssetManager = new AssetManager<CanvasItem>(SPRITEASSETJSONFILEPATH, LoadSprite);
        _spriteAssetManager.PreloadAssetGroup("forms");

        _unitTemplateAssetManager = new AssetManager<UnitTemplate>(UNITTEMPLATEASSETJSONFILEPATH, LoadUnitTemplate);
        _unitTemplateAssetManager.PreloadAll();
        //_effectVisualizationAssetManager = new AssetManager<PackedScene>(EFFECTVISUALIZATIONJSONFILEPATH);
    }

    public ModifierTemplate GetModifierTemplate(string name)
    {
        return _modifierTemplateLibrary.GetValueOrDefault(name);
    }

    public MoveTemplate GetMoveTemplate(string assetName)
    {
        return _moveTemplateLibrary.GetValueOrDefault(assetName);
    }

    public SpriteFont GetFont(string assetName)
    {
        return _fontLibrary.GetValueOrDefault(assetName);
    }

    public EffectVisualizationTemplate GetEffectVisualizationTemplate(string assetName)
    {
        return _effectVisualizationTemplateLibrary.GetValueOrDefault(assetName);
    }

    public Emotion GetEmotion(EmotionTag tag)
    {
        return _emotionLibrary.GetValueOrDefault(tag);
    }

    public TrackedResource<ColoredPolygon> GetPolygon(string assetName)
    {
        return _polygonAssetManager.GetAsset(assetName);
    }

    public TrackedResource<CanvasItem> GetSprite(string assetName)
    {
        return _spriteAssetManager.GetAsset(assetName);
    }

    public TrackedResource<UnitTemplate> GetUnitTemplate(string assetName)
    {
        return _unitTemplateAssetManager.GetAsset(assetName);
    }

    /// <summary>
	/// Loads visualization with stored visualization name for effect and all child effects
	/// </summary>
	/// <param name="template"></param>
	public Effect LoadEffectFromTemplate(EffectTemplate template)
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

    public List<Effect> LoadMultipleEffectsFromTemplates(IEnumerable<EffectTemplate> effectTemplates)
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

    public Func<string, UnitTemplate> LoadUnitTemplate;

    private UnitTemplate LoadUnitTemplateInternal(string assetPath)
    {
        DeserializedUnitTemplate dUnitTemplate = ReadJson<DeserializedUnitTemplate>(assetPath);

        if (dUnitTemplate == null)
            return null;

        int curHealth = dUnitTemplate.CurHealth;

        if (curHealth == -1)
            curHealth = dUnitTemplate.MaxHealth;

        Dictionary<StatType, int> statsList = new Dictionary<StatType, int>();
        statsList.Add(StatType.MaxHealth, dUnitTemplate.MaxHealth);
        statsList.Add(StatType.Attack, dUnitTemplate.Attack);
        statsList.Add(StatType.Defense, dUnitTemplate.Defense);
        statsList.Add(StatType.DecayRate, dUnitTemplate.DecayRate);
        statsList.Add(StatType.CurHealth, curHealth);
        statsList.Add(StatType.CurDecay, dUnitTemplate.CurDecay);

        ReadOnlyCollection<string> moveSetString = new ReadOnlyCollection<string>(dUnitTemplate.MoveSet);

        UnitTemplate unitTemplate = new UnitTemplate(
            new ReadOnlyDictionary<StatType, int>(statsList),
            moveSetString,
            (EmotionTag)dUnitTemplate.Emotion,
            dUnitTemplate.TimeOnBoard,
            dUnitTemplate.SpriteName,
            dUnitTemplate.FriendlyName);

        return unitTemplate;
    }

    public static Func<string, ColoredPolygon> LoadPolygon = (string assetPath)
        => LoadPolygonInternal(assetPath);

    private static ColoredPolygon LoadPolygonInternal(string assetPath)
    {
        DeserializedPolygon deserializedPolygon = ReadJson<DeserializedPolygon>(assetPath);

        Polygon polygon = null;
        if (deserializedPolygon.Vertices != null)
        {
            List<Vector2> vertices = deserializedPolygon.VerticesAsVectorList();
            polygon = new Polygon(vertices);
        }
        else if (deserializedPolygon.Radius != 0)
        {
            polygon = RegularPolygon(deserializedPolygon.Radius, deserializedPolygon.Sides);
        }


        ColoredPolygon coloredPolygon = new ColoredPolygon(polygon, deserializedPolygon.Color, deserializedPolygon.LineThickness, deserializedPolygon.Filled);

        return coloredPolygon;
    }

    public static Polygon RegularPolygon(float radius, int sides)
    {
        if (sides < 3)
            return null;

        if (radius < 0)
            return null;

        float rotationPer = Position.MAXROTATION / sides;

        List<Vector2> vertices = new List<Vector2>();

        for (int i = 0; i < sides; i++)
        {
            vertices.Add(Position.RotatePointAroundPoint(new Vector2(radius, 0), Vector2.Zero, i * rotationPer));
        }

        return new Polygon(vertices);
    }

    public Func<string, CanvasItem> LoadSprite;

    private CanvasItem LoadSpriteInternal(string assetPath)
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

        if (rules.Count == 0)
            return new CanvasItem(new Position(deserializedSprite.PositionArgs), null, boundingZones, unrulyChildren);

        return new CanvasItem_TransformationRules(rules, unrulyChildren, boundingZones, new Position(deserializedSprite.PositionArgs));
    }

    private CanvasItem InstantiateCanvasItem(string resourceName, string resourceType, float[] positionArgs, DeserializedBoundingZone[] deserializedboundingZones = null)
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
                newSprite.Set(new Position(positionArgs));

                return newSprite;

            case ("none"):
                return null;

            default:
                return null;
        }
    }

    private const bool SHOWBOUNDINGZONEOUTLINE = true;

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
        Position position = new Position(deserializedZone.PositionArgs);

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

    private static CanvasTransformationRule InstantiateCanvasTransformationRule(DeserializedCanvasTransformationRule dRule, CanvasItem canvasItem)
    {
        string type = dRule.TransformationType.ToLower();
        Func<float, float> velocityFunc = GetVelocityFunc(dRule.VelocityFunction);
        float duration = dRule.TransformationDuration;

        switch (duration)
        {
            case -7:
                duration = (float)UnitSprite.ATTACKANIMATIONDURATION; break;
            case -8:
                duration = (float)UnitSprite.HURTANIMATIONDURATION; break;
            case -9:
                duration = (float)UnitSprite.DEATHANIMATIONDURATION; break;
        }

        switch (type)
        {
            case ("rotation"):
                if (dRule.Transformation.Length < 3)
                    return null;

                Vector2 origin = new Vector2(dRule.Transformation[0], dRule.Transformation[1]);

                float peakRotationDiff = 0;
                
                if (dRule.PeakTransformation != null)
                    peakRotationDiff = dRule.PeakTransformation[2] - dRule.Transformation[2];

                return new CanvasTransformationRule_Rotation(
                    canvasItem,
                    dRule.ActiveStates,
                    dRule.Transformation[2],
                    peakRotationDiff,
                    origin,
                    duration,
                    velocityFunc);

            case ("translation"):
                if (dRule.Transformation.Length < 2)
                    return null;

                Vector2 translation = new Vector2(dRule.Transformation[0], dRule.Transformation[1]);

                Vector2 peakTranslationDiff = Vector2.Zero;

                if (dRule.PeakTransformation != null)
                    peakTranslationDiff = new Vector2(
                        dRule.PeakTransformation[0] - dRule.Transformation[0],
                        dRule.PeakTransformation[1] - dRule.Transformation[1]);

                return new CanvasTransformationRule_Translation(
                    canvasItem,
                    dRule.ActiveStates,
                    translation,
                    peakTranslationDiff,
                    duration,
                    velocityFunc);

            case ("vertexTranslation"):
                if (dRule.Transformation.Length < 2)
                    return null;

                if (dRule.VerticeIndices.Length == 0)
                    return null;

                translation = new Vector2(dRule.Transformation[0], dRule.Transformation[1]);

                peakTranslationDiff = Vector2.Zero;

                if (dRule.PeakTransformation != null)
                    peakTranslationDiff = new Vector2(
                        dRule.PeakTransformation[0] - dRule.Transformation[0],
                        dRule.PeakTransformation[1] - dRule.Transformation[1]);

                return new CanvasTransformationRule_VertexTranslation(
                    canvasItem,
                    dRule.ActiveStates,
                    dRule.VerticeIndices,
                    translation,
                    peakTranslationDiff,
                    duration,
                    velocityFunc);

            default: 
                return null;
        }
    }

    private static Func<float, float> GetVelocityFunc(string name)
    {
        if (name == null) return null;

        switch (name.ToLower())
        {
            case "smoothbell":
                return CanvasTransformationRule.SmoothBell;
            default:
                return null;
        }
    }

    public static T ReadJson<T>(string jsonPath)
    {
        jsonPath = FILEPREFIX + jsonPath;

        string jsonString = File.ReadAllText(jsonPath);

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

public interface IAssetLoadOnly
{
    public ModifierTemplate GetModifierTemplate(string name);
    public MoveTemplate GetMoveTemplate(string assetName);
    public SpriteFont GetFont(string assetName);
    public EffectVisualizationTemplate GetEffectVisualizationTemplate(string assetName);
    public Emotion GetEmotion(EmotionTag tag);
    public TrackedResource<ColoredPolygon> GetPolygon(string assetName);
    public TrackedResource<CanvasItem> GetSprite(string assetName);
    public TrackedResource<UnitTemplate> GetUnitTemplate(string assetName);
    public Effect LoadEffectFromTemplate(EffectTemplate template);
    public List<Effect> LoadMultipleEffectsFromTemplates(IEnumerable<EffectTemplate> effectTemplates);
}