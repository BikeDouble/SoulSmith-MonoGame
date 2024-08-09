using MonoGame.Extended.Shapes;
using Microsoft.Xna.Framework;

public class ColoredPolygon
{
    public Polygon Polygon;
    public Color Color;
    public bool Filled;
    public int LineThickness;

    public ColoredPolygon(ColoredPolygon other)
    {
        Polygon = DeepCopy(other.Polygon);
        Color = other.Color;
    }

    public ColoredPolygon(Polygon polygon, int[] color, int lineThickness, bool filled) 
    {
        if ((color == null) || (color?.Length < 4))
        {
            Color = Color.White;
        }
        else
        {
            Color = new Color(color[0], color[1], color[2], color[3]);
        }

        Polygon = DeepCopy(polygon);
        Filled = filled;
        LineThickness = lineThickness;
    }

    public ColoredPolygon(Polygon polygon, Color color, int lineThickness, bool filled)
    {
        Color = color;
        Polygon = DeepCopy(polygon);
        Filled = filled;
        LineThickness = lineThickness;
    }

    public static Polygon DeepCopy(Polygon otherPolygon)
    {
        if (otherPolygon is null) 
            return null;

        Vector2[] vertices = new Vector2[otherPolygon.Vertices.Length];

        for (int i = 0; i < otherPolygon.Vertices.Length; i++)
        {
            vertices[i] = new Vector2(otherPolygon.Vertices[i].X, otherPolygon.Vertices[i].Y);
        }

        return new Polygon(vertices);
    }
}

