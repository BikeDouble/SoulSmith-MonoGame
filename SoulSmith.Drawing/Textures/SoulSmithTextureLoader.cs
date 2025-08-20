using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Drawing.Zoned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Textures
{
    public class SoulSmithTextureLoader : IGraphicsAssetLoader
    {
        public IDisposable Load(string path, GraphicsDevice graphics)
        {
            if (!File.Exists(path)) return null;

            Texture2D texture = Texture2D.FromFile(graphics, path);

            string jsonPath = Path.ChangeExtension(path, ".meta.json");

            SoulSmithTextureMetaData metaData = null;

            if (File.Exists(jsonPath))
            {
                metaData = JsonSerializer.Deserialize<SoulSmithTextureMetaData>(File.ReadAllText(jsonPath));
            }

            SoulSmithTexture soulSmithTexture = new SoulSmithTexture(texture, metaData);

            return soulSmithTexture;
        }
    }
}
