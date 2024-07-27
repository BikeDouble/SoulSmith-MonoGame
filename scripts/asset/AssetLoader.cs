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
        List<Vector2> vertices = deserializedPolygon.VerticesAsVectorList();
        Polygon polygon = new Polygon(vertices);
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

        foreach (DeserializedSpritePart part in deserializedParts) 
        {
            CanvasItem child = InstantiateCanvasItem(part.ResourceName, part.ResourceType);

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

        return new CanvasItem_TransformationRules(rules, unrulyChildren);
    }

    private CanvasItem InstantiateCanvasItem(string resourceName, string resourceType)
    {
        resourceType = resourceType.ToLower();

        switch (resourceType)
        {
            case ("polygon"):
                TrackedResource<ColoredPolygon> polygon = GetPolygonAsset(resourceName);

                if (polygon == null)
                    return null;

                return new CanvasItem(polygon);

            /*case ("texture"):
                TrackedResource<Texture2D> texture = GetTextureAsset(resourceName);

                if (texture == null)
                    return null;

                return new CanvasItem(texture);*/

            case ("sprite"):
                TrackedResource<CanvasItem_TransformationRules> sprite = GetSpriteAsset(resourceName);

                if (sprite == null) 
                    return null;

                return new CanvasItem_TransformationRules(sprite);

            case ("none"):
                return null;

            default:
                return null;
        }
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

    private CanvasItem CreateCanvasItemWithUncertainType(string assetName, string assetType)
    {
        assetType = assetType.ToLower();

        switch (assetType)
        {
            case ("polygon"):
                return new CanvasItem(GetPolygonAsset(assetName));

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

