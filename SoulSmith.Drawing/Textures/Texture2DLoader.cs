using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Textures
{
    public class Texture2DLoader : IGraphicsAssetLoader
    {
        public IDisposable Load(string path, GraphicsDevice graphics)
        {
            if (!File.Exists(path)) return null;

            Texture2D texture = Texture2D.FromFile(graphics, path);

            return texture;
        }
    }
}
