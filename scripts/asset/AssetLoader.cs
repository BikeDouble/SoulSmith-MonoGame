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


public class AssetLoader
{
    //Im bad at files
    public const string FILEPREFIX = "../../../";

    private readonly ContentManager _content;
    private readonly GameManager _gameManager;

    public AssetLoader(ContentManager content, GameManager gameManager)
    {
        _content = content;
        _gameManager = gameManager;

        LoadSprite = (string assetPath)
            => LoadSpriteInternal(assetPath);

        LoadUnitTemplate = (string assetPath)
            => LoadUnitTemplateInternal(assetPath);
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


        ColoredPolygon coloredPolygon = new ColoredPolygon(polygon, deserializedPolygon.Color);

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

    public Func<string, CanvasItem_TransformationRules> LoadSprite;

    private CanvasItem_TransformationRules LoadSpriteInternal(string assetPath)
    {
        DeserializedSprite deserializedSprite = ReadJson<DeserializedSprite>(assetPath);
        DeserializedSpritePart[] deserializedParts = deserializedSprite.Parts;

        List<CanvasTransformationRule> rules = new List<CanvasTransformationRule>();
        List<FormsObject> unrulyChildren = new List<FormsObject>();
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
            CanvasItem child = InstantiateCanvasItem(part.ResourceName, part.ResourceType, part.BoundingZones);

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

            if (child.BoundingZones != null)
            {
                foreach (KeyValuePair<BoundingZoneType, CanvasItem> item in  child.BoundingZones)
                {
                    boundingZones.TryAdd(item.Key, child);
                }
            }
        }

        return new CanvasItem_TransformationRules(rules, unrulyChildren, boundingZones);
    }

    private CanvasItem InstantiateCanvasItem(string resourceName, string resourceType, DeserializedBoundingZone[] deserializedboundingZones = null)
    {
        resourceType = resourceType.ToLower();
        Dictionary<BoundingZoneType, CanvasItem> boundingZones;

        switch (resourceType)
        {
            case ("polygon"):
                TrackedResource<ColoredPolygon> polygon = GetPolygonAsset(resourceName);

                if (polygon == null)
                    return null;

                boundingZones = InstantiateBoundingZones(deserializedboundingZones);
                return new CanvasItem(new DrawableResource_Polygon(polygon), boundingZones);

            /*case ("texture"):
                TrackedResource<Texture2D> texture = GetTextureAsset(resourceName);

                if (texture == null)
                    return null;

                return new CanvasItem(texture);*/

            case ("sprite"):
                TrackedResource<CanvasItem_TransformationRules> sprite = GetSpriteAsset(resourceName);

                if (sprite == null) 
                    return null;

                return (CanvasItem_TransformationRules)sprite.Resource.DeepClone();

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

        switch (type)
        {
            case ("rotation"):
                if (dRule.Transformation.Length < 3)
                    return null;

                Vector2 origin = new Vector2(dRule.Transformation[0], dRule.Transformation[1]);

                return new CanvasTransformationRule_Rotation(
                    canvasItem,
                    dRule.ActiveStates,
                    dRule.Transformation[2],
                    origin,
                    dRule.TransformationDuration,
                    dRule.Acceleration);

            case ("translation"):
                if (dRule.Transformation.Length < 2)
                    return null;

                Vector2 translation = new Vector2(dRule.Transformation[0], dRule.Transformation[1]);

                return new CanvasTransformationRule_Translation(
                    canvasItem,
                    dRule.ActiveStates,
                    translation,
                    dRule.TransformationDuration,
                    dRule.Acceleration);

            case ("vertexTranslation"):
                if (dRule.Transformation.Length < 2)
                    return null;

                if (dRule.VerticeIndices.Length == 0)
                    return null;

                translation = new Vector2(dRule.Transformation[0], dRule.Transformation[1]);

                return new CanvasTransformationRule_VertexTranslation(
                    canvasItem,
                    dRule.ActiveStates,
                    dRule.VerticeIndices,
                    translation,
                    dRule.TransformationDuration,
                    dRule.Acceleration);

            default: 
                return null;
        }
    }

    private TrackedResource<ColoredPolygon> GetPolygonAsset(string assetName)
    {
        return _gameManager.GetPolygonAsset(assetName);
    }

    private TrackedResource<CanvasItem_TransformationRules> GetSpriteAsset(string assetName)
    {
        return _gameManager.GetSpriteAsset(assetName);
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

