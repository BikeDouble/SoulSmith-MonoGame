using SoulSmith.Core;
using Microsoft.Xna.Framework;

namespace SoulSmith.Shapes
{
    public interface IMultiZone : IZone
    {
        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation, string zoneKey);
        public bool ContainsLocal(Vector2 point, string zoneKey);
        public Vector2 GetRandomLocalPoint(string zoneKey);
        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position, string zoneKey);
        public float GetAreaLocal(string zoneKey);
    }
}
