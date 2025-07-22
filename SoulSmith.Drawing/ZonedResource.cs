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

namespace SoulSmith.Drawing
{
    [JsonConverter(typeof(ZonedResourceJsonConverter))]
    public class ZonedResource : IZonedResource, IMultiZone, IAsset
    {
        private IZone _zone;
        private IAssetWrapper<IDrawableResource> _resource;

        public ZonedResource(IZone zone, IAssetWrapper<IDrawableResource> resource) //TODO add multizone support
        {
            _zone = zone;
            _resource = resource;
        }
        
        public void Process(double delta)
        {
            _resource?.Value?.Process(delta);
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation = null)
        {
            return ContainsGlobal(point, transformation, string.Empty);
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation, string zoneKey)
        {
            if (_zone == null) return false;

            Position newTransformation = new Position(transformation);

            newTransformation.Translate(Origin * transformation.ScaleVector * -1);

            return _zone.ContainsGlobal(point, newTransformation);
        }

        public bool ContainsLocal(Vector2 point)
        {
            return ContainsLocal(point, string.Empty);
        }

        public bool ContainsLocal(Vector2 point, string zoneKey)
        {
            if (_zone == null) { return false; }

            Position originTransformation = new Position(Origin * -1);

            return _zone.ContainsGlobal(point, originTransformation);
        }

        public Vector2 GetRandomLocalPoint()
        {
            return GetRandomLocalPoint(string.Empty);
        }

        public Vector2 GetRandomLocalPoint(string zoneKey)
        {
            return _zone.GetRandomLocalPoint();
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position)
        {
            return GetRandomGlobalPoint(position, string.Empty);
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position, string zoneKey)
        {
            return _zone.GetRandomGlobalPoint(position);
        }

        public float GetAreaLocal()
        {
            return GetAreaLocal(string.Empty);
        }

        public float GetAreaLocal(string zoneKey)
        {
            return _zone.GetAreaLocal();
        }

        public float GetHeightLocal()
        {
            return GetHeightLocal(string.Empty);
        }

        public float GetHeightLocal(string zoneKey)
        {
            return _zone.GetHeightLocal();
        }

        public float GetWidthLocal()
        {
            return GetWidthLocal(string.Empty);
        }

        public float GetWidthLocal(string zoneKey)
        {
            return _zone.GetWidthLocal();
        }

        public void Draw(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch)
        {
            _resource?.Value?.Draw(position, color, spriteBatch);
        }

        public void Dispose() 
        {
            _resource.Dispose();
        }

        public int Width { get { return _resource.Value.Width; } }
        public int Height { get { return _resource.Value.Height; } }
        public Vector2 Origin { get { return _resource.Value.Origin; } }
    }

    public class ZonedResourceJsonConverter : JsonConverter<ZonedResource>
    {
        public override ZonedResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            IAssetWrapper<IDrawableResource> resource = null;
            IZone zone = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Zone":
                        zone = JsonSerializer.Deserialize<IZone>(ref reader, options);
                        reader.Read();
                        break;
                    case "Resource":
                    case "DrawableResource":
                        TrackedIDrawableResourceJsonConverter converter = new TrackedIDrawableResourceJsonConverter();
                        resource = converter.Read(ref reader, typeof(IAssetWrapper<IDrawableResource>), options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new ZonedResource(zone, resource);
        }

        public override void Write(Utf8JsonWriter writer, ZonedResource value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
