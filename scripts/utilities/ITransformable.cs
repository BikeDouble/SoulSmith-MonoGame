using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

public interface ITransformable : IDeepCloneable
{
    void Set(CanvasPosition position);
    void Set(Vector2 coordinates);
    void Transform(CanvasPosition transformation);
    void Translate(Vector2 translation);
    void ScaleMultiplicative(Vector2 scale);
    void ScaleAdditive(Vector2 scale);
    void Rotate(float rotation);
    void Rotate(float rotation, Vector2 origin);
    void ChangeTintAdditive(float r, float g, float b, float a);
    void ChangeTintAdditive(Vector4 change);
    IReadOnlyCanvasPosition Position { get; }
}

