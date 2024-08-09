using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class DeserializedPolygon
{
    public DeserializedVertex[] Vertices { get; set; } = null;
    public int[] Color { get; set; }
    public int LineThickness { get; set; } = 1;
    public bool Filled { get; set; } = true;
    public float Radius { get; set; } = 0f;
    public int Sides { get; set; } = 3;

    public List<Vector2> VerticesAsVectorList()
    {
        List<Vector2> result = new List<Vector2>();

        foreach (DeserializedVertex vertex in Vertices)
        {
            result.Add(new Vector2(vertex.X, vertex.Y));
        }

        return result;
    }
}

public class DeserializedVertex
{
    public float X { get; set; }
    public float Y { get; set; }
}
