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
    public class ZonedResource : IZone, IDrawableResource, IAsset
    {
        private IZone _clickZone;
        private IAssetWrapper<IDrawableResource> _resource;

        public ZonedResource(IZone clickZone, IAssetWrapper<IDrawableResource> resource)
        {
            _clickZone = clickZone;
            _resource = resource;
        }
        
        public void Process(double delta)
        {
            _resource?.Value?.Process(delta);
        }

        public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation = null)
        {
            if (_clickZone == null) return false;

            Position newTransformation = new Position(transformation);

            newTransformation.Translate(Origin * transformation.ScaleVector * -1);

            return _clickZone.ContainsGlobal(point, newTransformation);
        }

        public bool ContainsLocal(Vector2 point)
        {
            if (_clickZone == null) { return false; }

            Position originTransformation = new Position(Origin * -1);

            return _clickZone.ContainsGlobal(point, originTransformation);
        }

        public Vector2 GetRandomLocalPoint()
        {
            return _clickZone.GetRandomLocalPoint();
        }

        public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position)
        {
            return _clickZone.GetRandomGlobalPoint(position);
        }

        public float GetAreaLocal()
        {
            return _clickZone.GetAreaLocal();
        }

        public void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch)
        {
            _resource?.Value?.Draw(position, tint, spriteBatch);
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
