using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;

namespace SoulSmith.Drawing
{
    public class Texture2DResourceLoader : IGraphicsAssetLoader
    {
        public IAsset Load(string path, GraphicsDevice graphics)
        {
            if (!File.Exists(path)) return null;

            Texture2D texture = Texture2D.FromFile(graphics, path);

            if (texture == null) return null;

            Texture2DResource resource = new Texture2DResource(texture);

            return resource;
        }
    }
}
