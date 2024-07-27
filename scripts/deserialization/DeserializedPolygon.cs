using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class DeserializedPolygon
{
    public DeserializedVertex[] Vertices { get; set; }
    public int[] Color { get; set; }

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
