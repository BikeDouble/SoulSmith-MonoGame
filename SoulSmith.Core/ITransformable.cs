using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SoulSmith.Core;

namespace SoulSmith.Core
{
    public interface ITransformable
    {
        void Set(Position position);
        void Set(Vector2 coordinates);
        void Transform(IReadOnlyPosition transformation);
        void Translate(Vector2 translation);
        void Scale(Vector2 scale);
        void Rotate(float rotation, Vector2? origin = null);
    }
}
