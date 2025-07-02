using Microsoft.Xna.Framework;
using SoulSmith.Core;
using System.Text.Json;
using System.Text.Json.Serialization;
using SoulSmith.Vector;
using SoulSmith.Collections;

namespace SoulSmith.Shapes
{
    [JsonConverter(typeof(PolygonJsonConverter))]
    public class Polygon : IZone, IDeepCloneable
    {
        private List<Vector2> _vertices;
        private bool? _counterClockwise = null;

        public Polygon(IEnumerable<Vector2> vertices)
        {
            _vertices = new List<Vector2>(vertices);
            _counterClockwise = IsCounterClockwise();
        }

        public bool IsClockwise()
        {
            return !IsCounterClockwise();
        }

        public bool IsCounterClockwise()
        {
            if (_counterClockwise.HasValue) return _counterClockwise.Value;

            float signedArea = GetSignedAreaLocal();

            return signedArea > 0;
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation)
        {
            if (transformation != null) return GetTransformedCopy(transformation).ContainsLocal(point);

            return ContainsLocal(point);
        }

        public bool ContainsLocal(Vector2 point)
        {
            if (_vertices.Count < 3) return false;

            return CountRayCastIntersections(point) % 2 == 1;
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition transformation)
        {
            if (transformation != null) return GetTransformedCopy(transformation).GetRandomLocalPoint();

            return GetRandomLocalPoint();
        }

        public Vector2 GetRandomLocalPoint()
        {
            if (_vertices.Count < 3) throw new Exception("Polygon contains less than three points.");

            if (_vertices.Count == 3) GetRandomLocalPointFromTriangle();

            List<WeightedListItem<Polygon>> weightedTriangles = new();

            List<Polygon> triangles = Triangulate();

            List<(Polygon, float)> trianglesWithArea = new();

            foreach (Polygon triangle in triangles) trianglesWithArea.Add((triangle, triangle.GetAreaLocal()));

            float totalArea = GetAreaLocal();

            foreach ((Polygon Triangle, float Area) item in trianglesWithArea)
            {
                WeightedListItem<Polygon> weightedListItem = 
                    new WeightedListItem<Polygon>(item.Triangle, (int)(item.Area/totalArea * 1000000));

                weightedTriangles.Add(weightedListItem);
            }

            SoulSmithWeightedList<Polygon> weightedList = new(weightedTriangles, Rand.Random);

            Polygon chosenTriangle = weightedList.Next();

            return chosenTriangle.GetRandomLocalPoint();
        }

        private Vector2 GetRandomLocalPointFromTriangle()
        {
            // Using barycentric coordinates

            if (_vertices.Count != 3) throw new Exception("Polygon is not a triangle.");

            float u = Rand.RandFloat();
            float v = Rand.RandFloat();

            if (u + v > 1)
            {
                u = 1 - u;
                v = 1 - v;
            }

            Vector2 point = _vertices[0] + u * (_vertices[1] - _vertices[0]) + v * (_vertices[2] - _vertices[0]);

            return point;
        }

        public float GetAreaLocal()
        { 
            float signedArea = GetSignedAreaLocal();

            return MathF.Abs(signedArea);
        }

        private float GetSignedAreaLocal()
        {
            // Shoelace Theorem
            int n = _vertices.Count;
            float sum = 0;

            for (int i = 0; i < n; i++)
            {
                Vector2 current = _vertices[i];
                Vector2 next = _vertices[(i + 1) % n];

                sum += (current.X * next.Y) - (next.X * current.Y);
            }

            return sum * 0.5f;
        }

        /// <summary>
        /// Returns a list of triangles that together form the polygon
        /// </summary>
        /// <returns></returns>
        private List<Polygon> Triangulate()
        {
            if (_vertices.Count <= 2) return null;

            List<Vector2> vertices = new List<Vector2>(_vertices);
            List<Polygon> result = new();

            while (vertices.Count > 3)
            {
                bool earFound = false;

                for (int i = 0; i < vertices.Count; i++)
                {
                    int prevIdx = (i - 1 + vertices.Count) % vertices.Count;
                    Vector2 previous = vertices[prevIdx];
                    Vector2 current = vertices[i];
                    int nextIdx = (i + 1) % vertices.Count;
                    Vector2 next = vertices[nextIdx];
                    Polygon curTriangle = new Polygon([previous, current, next]);

                    if (IsConvex(previous, current, next) && !curTriangle.ContainsAny(vertices, [previous, current, next]))
                    {
                        result.Add(curTriangle);
                        vertices.RemoveAt(i);
                        earFound = true;
                        break;
                    }
                }

                if (!earFound) throw new Exception("Polygon may be malformed or self-intersecting.");
            }

            result.Add(new Polygon([vertices[0], vertices[1], vertices[2]]));
            return result;
        }

        public bool IsConvex(Vector2 left, Vector2 middle, Vector2 right)
        {
            Vector2 leftEdge = middle - left;
            Vector2 rightEdge = right - middle;

            float crossProduct = leftEdge.X * rightEdge.Y + leftEdge.Y * rightEdge.X;

            if (IsCounterClockwise()) return crossProduct > 0;
            else return crossProduct < 0;
        }

        /// <summary>
        /// Returns true if the polygon contains one or more of the vertices
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public bool ContainsAny(IEnumerable<Vector2> vertices, IEnumerable<Vector2> ignoredVertices)
        {
            foreach (Vector2 vertex in vertices)
            {
                if (!ignoredVertices.Contains(vertex))
                {
                    if (ContainsLocal(vertex)) return true;
                }
            }

            return false;
        }

        private int CountRayCastIntersections(Vector2 point)
        {
            int intersections = 0;

            for (int i = 0; i < _vertices.Count; i ++)
            {
                Vector2 vertex1 = _vertices[i];

                Vector2 vertex2 = (i == _vertices.Count - 1) ? _vertices[0] : _vertices[i + 1];

                if (vertex1.Y != vertex2.Y)
                {
                    if ((vertex1.Y > point.Y) != (vertex2.Y > point.Y))
                    {
                        float intersectX = CalculateIntersectX(vertex1, vertex2, point.Y);

                        if (intersectX > point.X) intersections++;
                    }
                }
            }
            return intersections;
        }

        private static float CalculateIntersectX(Vector2 a, Vector2 b, float lineY)
        {
            float numerator = (lineY - a.Y) * (b.X - a.X);
            float denominator = (b.Y - a.Y);

            return a.X + numerator / denominator;
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

        public object DeepClone()
        {
            return new Polygon(_vertices);
        }

        public Polygon GetTransformedCopy(IReadOnlyPosition transformation)
        {
            Polygon copy = (Polygon)DeepClone();

            copy.Transform(transformation);

            return copy;
        }

        private void Transform(IReadOnlyPosition transformation)
        {
            Scale(transformation.ScaleVector);
            Translate(transformation.Coordinates);
            Rotate(transformation.Rotation);
        }

        private void Translate(Vector2 translation)
        {
            if (translation == Vector2.Zero) return;

            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = _vertices[i] + translation;
            }
        }

        private void Scale(Vector2 scale)
        {
            if (scale == Vector2.One) return;

            // Flip traversal direction if multiplied by one-dimension negative scale
            if ((scale.X < 0) ^ (scale.Y < 0))
            {
                _counterClockwise = !_counterClockwise;
            }

            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = _vertices[i] * scale;
            }
        }

        private void Rotate(float rotation, Vector2? origin = null)
        {
            if (rotation == 0) return;

            Vector2 originVal = origin ?? Vector2.Zero;

            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = Position.RotatePointAroundPoint(_vertices[i], originVal, rotation);
            }
        }
    }

    internal class PolygonJsonConverter : System.Text.Json.Serialization.JsonConverter<Polygon>
    {
        public override Polygon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of object");
            reader.Read();
            string text = reader.GetString() ?? throw new JsonException("String cannot be null");

            bool foundVertices = false;
            bool foundRadius = false;
            Polygon polygon = null;
            int sides = 0;
            float radius = 0;
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                string propertyName = reader.GetString() ?? throw new JsonException("Property name cannot be null");
                reader.Read();

                switch (propertyName)
                {
                    case "Vertices":
                        if (!foundVertices)
                        {
                            var serializationOptionsWithConverter = new JsonSerializerOptions();
                            serializationOptionsWithConverter.Converters.Add(new Vector2ArrayConverter());
                            Vector2[] vertices = JsonSerializer.Deserialize<Vector2[]>(ref reader, serializationOptionsWithConverter);

                            polygon = new Polygon(vertices);
                            foundVertices = true;
                        }
                        break;
                    case "Radius":
                        if (!foundRadius)
                        {
                            radius = reader.GetSingle();
                            foundRadius = true;
                        }
                        break;
                    case "Sides":
                        sides = reader.GetInt32();
                        break;
                    default:
                        throw new JsonException($"Unknown property: {propertyName}");
                }
                reader.Read();
            }

            if (polygon is null)
            {
                if (foundRadius && (sides > 0) && (radius > 0))
                {
                    polygon = Polygon.RegularPolygon(radius, sides);
                }
                else
                {
                    throw new JsonException("Vertices not defined");
                }
            }

            return polygon;
        }

        public override void Write(Utf8JsonWriter writer, Polygon value, JsonSerializerOptions options)
        {
            //TODO
        }
    }

    internal class ColorConverter : JsonConverter<Color> //TODO remove
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected StartArray token");

            float[] floats = new float[4];

            int i = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType != JsonTokenType.Number)
                    throw new JsonException("Expected Number");

                floats[i] = reader.GetSingle();
            }

            return new Color(floats[0], floats[1], floats[2], floats[3]);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            return; //TODO
        }
    }
}
