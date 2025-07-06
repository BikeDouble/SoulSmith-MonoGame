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

            ZonedResource resource = JsonSerializer.Deserialize<ZonedResource>(File.ReadAllText(path));

            return resource;
        }
    }
}
