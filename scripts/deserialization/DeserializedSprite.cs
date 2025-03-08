namespace SoulSmithDeserialization;
public class DeserializedSprite
{
    public DeserializedCanvasTransformationRule[] MovementRules { get; set; } = null;
    public DeserializedSpritePart[] Parts { get; set; }
    public DeserializedBoundingZone[] BoundingZones { get; set; } = null;
    public float[] PositionArgs { get; set; } = null;
    public string ShellName { get; set; } = null;
}

public class DeserializedSpritePart
{
    public string ResourceName { get; set; }
    public string ResourceType { get; set; }
    public DeserializedCanvasTransformationRule[] MovementRules { get; set; } = null;
    public DeserializedBoundingZone[] BoundingZones { get; set; } = null;
    public float[] PositionArgs { get; set; } = null;
}

public class DeserializedCanvasTransformationRule
{
    public int[] ActiveStates { get; set; }
    public int[] VerticeIndices { get; set; }
    public string TransformationType { get; set; }
    public float TransformationDuration { get; set; }
    public string VelocityFunction { get; set; } = null;
    public float[] Transformation { get; set; }
    public float[] PeakTransformation { get; set; } = null;
    public bool PeakIsTarget { get; set; } = false;
    public float CycleDuration { get; set; } = -1;
    public string Tag { get; set; } = string.Empty;
}

public class DeserializedBoundingZone
{
    public string Shape { get; set; }
    public string[] Types { get; set; } = null;
    public float[] ZoneArgs { get; set; }
    public float[] PositionArgs { get; set; } = null;
}

public class DeserializedSpriteShell
{
    public DeserializedCanvasTransformationRule[] MovementRules { get; set; } = null;
}

public class DeserializedSpriteAndShell
{
    public string SpriteName { get; set; }
    public string ShellName { get; set; } = null;
}