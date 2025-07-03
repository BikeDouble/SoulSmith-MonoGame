using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Drawing
{
    public class ZonedResourceLoader : IGraphicsAssetLoader
    {
        public IAsset Load(string path, GraphicsDevice graphics = null)
        {
            if (!File.Exists(path)) return null;

            Texture2D texture = Texture2D.FromFile(graphics, path);

            if (texture == null) return null;

            string metaFilepath = AssetManager.GetMetaFilepathFromFilepath(path, "json"); 

            if (!File.Exists(metaFilepath)) return null;

            IZone zone = JsonSerializer.Deserialize<IZone>(File.ReadAllText(metaFilepath));

            if (zone == null) return null;

            ZonedResource resource = new ZonedResource(zone, new DrawableResource_Texture2D(texture));

            return resource;
        }
    }
}
