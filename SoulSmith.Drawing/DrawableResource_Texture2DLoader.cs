using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;

namespace SoulSmith.Drawing
{
    public class DrawableResource_Texture2DLoader : IGraphicsAssetLoader
    {
        public IAsset Load(string path, GraphicsDevice graphics)
        {
            if (!File.Exists(path)) return null;

            Texture2D texture = Texture2D.FromFile(graphics, path);

            if (texture == null) return null;

            DrawableResource_Texture2D resource = new DrawableResource_Texture2D(texture);

            return resource;
        }
    }
}
