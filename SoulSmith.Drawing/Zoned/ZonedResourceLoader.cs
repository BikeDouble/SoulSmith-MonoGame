using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Zoned
{
    public class ZonedResourceLoader : IBasicAssetLoader
    {
        public IDisposable Load(string path)
        {
            if (!File.Exists(path)) return null;

            ZonedResource resource = JsonSerializer.Deserialize<ZonedResource>(File.ReadAllText(path));

            return resource;
        }
    }
}
