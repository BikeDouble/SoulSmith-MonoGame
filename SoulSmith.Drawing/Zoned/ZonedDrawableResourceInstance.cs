using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Shapes;

namespace SoulSmith.Drawing.Zoned
{
    [JsonConverter(typeof(ZonedDrawableResourceInstanceJsonConverter))]
    public class ZonedDrawableResourceInstance : IMultiZone, IDisposable, IDrawableResource
    {
        private Dictionary<string, IZone> _zones;
        private IDrawableResource _resource;

        public SamplerState SamplerState { get { return _resource.SamplerState; } }

        public ZonedDrawableResourceInstance(IDictionary<string, IZone> zones, IDrawableResource resource) //TODO add multizone support
        {
            _zones = new Dictionary<string, IZone>(zones);
            _resource = resource;
        }
        
        public void Process(double delta)
        {
            _resource?.Process(delta);
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation = null)
        {
            return ContainsGlobal(point, transformation, string.Empty);
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation, string zoneKey)
        {
            IZone zone = _zones.ContainsKey(zoneKey) ? _zones[zoneKey] : GetDefaultZone();

            if (zone == null) return false;

            Position newTransformation = new Position(transformation);

            newTransformation.Translate(Origin * transformation.ScaleVector * -1);

            return zone.ContainsGlobal(point, newTransformation);
        }

        public bool ContainsLocal(Vector2 point)
        {
            return ContainsLocal(point, string.Empty);
        }

        public bool ContainsLocal(Vector2 point, string zoneKey)
        {
            IZone zone = _zones.ContainsKey(zoneKey) ? _zones[zoneKey] : GetDefaultZone();

            if (zone == null) { return false; }

            Position originTransformation = new Position(Origin * -1);

            return zone.ContainsGlobal(point, originTransformation);
        }

        public Vector2 GetRandomLocalPoint()
        {
            return GetRandomLocalPoint(string.Empty);
        }

        public Vector2 GetRandomLocalPoint(string zoneKey)
        {
            IZone zone = _zones.ContainsKey(zoneKey) ? _zones[zoneKey] : GetDefaultZone();

            return zone.GetRandomLocalPoint();
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position)
        {
            return GetRandomGlobalPoint(position, string.Empty);
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position, string zoneKey)
        {
            IZone zone = _zones.ContainsKey(zoneKey) ? _zones[zoneKey] : GetDefaultZone();

            return zone.GetRandomGlobalPoint(position);
        }

        public float GetAreaLocal()
        {
            return GetAreaLocal(string.Empty);
        }

        public float GetAreaLocal(string zoneKey)
        {
            IZone zone = _zones.ContainsKey(zoneKey) ? _zones[zoneKey] : GetDefaultZone();

            return zone.GetAreaLocal();
        }

        public float GetHeightLocal()
        {
            return GetHeightLocal(string.Empty);
        }

        public float GetHeightLocal(string zoneKey)
        {
            IZone zone = _zones.ContainsKey(zoneKey) ? _zones[zoneKey] : GetDefaultZone();

            return zone.GetHeightLocal();
        }

        public float GetWidthLocal()
        {
            return GetWidthLocal(string.Empty);
        }

        public float GetWidthLocal(string zoneKey)
        {
            IZone zone = _zones.ContainsKey(zoneKey) ? _zones[zoneKey] : GetDefaultZone();

            return zone.GetWidthLocal();
        }

        public void Draw(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch)
        {
            _resource?.Draw(position, color, spriteBatch);
        }

        public void Dispose() 
        {
            _resource.Dispose();
        }

        public void UpdateState(string newState, bool force = false)
        {
            _resource.UpdateState(newState, force);
        }

        private IZone GetDefaultZone()
        {
            if (_zones.Count == 0) return null;
            return _zones.Values.FirstOrDefault();
        }

        public int Width { get { return _resource.Width; } }
        public int Height { get { return _resource.Height; } }
        public Vector2 Origin { get { return _resource.Origin; } }
        public OriginPlacement OriginPlacement { get { return _resource.OriginPlacement; } set { _resource.OriginPlacement = value; } }
    }

    public class ZonedDrawableResourceInstanceJsonConverter : JsonConverter<ZonedDrawableResourceInstance>
    {
        public override ZonedDrawableResourceInstance Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            IDrawableResource resource = null;
            Dictionary<string, IZone> zones = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Zones":
                    case "zones":
                        ZoneDictionaryJsonConverter zoneConverter = new ZoneDictionaryJsonConverter();
                        zones = zoneConverter.Read(ref reader, typeof(Dictionary<string, IZone>), options);
                        reader.Read();
                        break;
                    case "Resource":
                    case "DrawableResource":
                        string drawableResourceKey = reader.GetString();
                        resource = DrawHelpers.GetDrawableResourceInstance(drawableResourceKey);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (zones == null || zones.Count == 0) throw new JsonException("Zones cannot be null or empty");
            if (resource == null) throw new JsonException("Resource cannot be null");

            return new ZonedDrawableResourceInstance(zones, resource);
        }

        public override void Write(Utf8JsonWriter writer, ZonedDrawableResourceInstance value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
