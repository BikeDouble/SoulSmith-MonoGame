using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using SoulSmith.Core;

namespace SoulSmith.Shapes
{
    public class MultiZone : IMultiZone
    {
        private ReadOnlyDictionary<string, IZone> _zones;

        public MultiZone(IDictionary<string, IZone> zones)
        {
            _zones = new ReadOnlyDictionary<string, IZone>(zones);
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition position, string key)
        {
            if (!_zones.ContainsKey(key)) return false;
            IZone zone = _zones[key];
            return zone.ContainsGlobal(point, position);
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition position)
        {
            IZone zone = DefaultZone;
            if (zone == null) return false;
            return zone.ContainsGlobal(point, position);
        }

        public bool ContainsLocal(Vector2 point, string key)
        {
            if (!_zones.ContainsKey(key)) return false;
            IZone zone = _zones[key];
            return zone.ContainsLocal(point);
        }

        public bool ContainsLocal(Vector2 point)
        {
            IZone zone = DefaultZone;
            if (zone == null) return false;
            return zone.ContainsLocal(point);
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position, string key)
        {
            if (!_zones.ContainsKey(key)) return Vector2.Zero;
            IZone zone = _zones[key];
            return zone.GetRandomGlobalPoint(position);
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position)
        {
            IZone zone = DefaultZone;
            if (zone == null) return Vector2.Zero;
            return zone.GetRandomGlobalPoint(position);
        }

        public Vector2 GetRandomLocalPoint(string key)
        {
            if (!_zones.ContainsKey(key)) return Vector2.Zero;
            IZone zone = _zones[key];
            return zone.GetRandomLocalPoint();
        }

        public Vector2 GetRandomLocalPoint()
        {
            IZone zone = DefaultZone;
            if (zone == null) return Vector2.Zero;
            return zone.GetRandomLocalPoint();
        }

        public float GetAreaLocal(string key)
        {
            if (!_zones.ContainsKey(key)) return 0f;
            IZone zone = _zones[key];
            return zone.GetAreaLocal();
        }

        public float GetAreaLocal()
        {
            IZone zone = DefaultZone;
            if (zone == null) return 0f;
            return zone.GetAreaLocal();
        }

        public float GetWidthLocal(string key)
        {
            if (!_zones.ContainsKey(key)) return 0f;
            IZone zone = _zones[key];
            return zone.GetWidthLocal();
        }

        public float GetWidthLocal()
        {
            IZone zone = DefaultZone;
            if (zone == null) return 0f;
            return zone.GetWidthLocal();
        }

        public float GetHeightLocal(string key)
        {
            if (!_zones.ContainsKey(key)) return 0f;
            IZone zone = _zones[key];
            return zone.GetHeightLocal();
        }

        public float GetHeightLocal()
        {
            IZone zone = DefaultZone;
            if (zone == null) return 0f;
            return zone.GetHeightLocal();
        }

        private IZone DefaultZone { get { return _zones.Values.FirstOrDefault(); } }
    }
}
