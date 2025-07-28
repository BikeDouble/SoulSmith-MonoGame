using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Drawing.Animation;
using SoulSmith.Drawing.Text;
using SoulSmith.Drawing.Textures;
using SoulSmith.Drawing.Zoned;
using System.Reflection.Metadata.Ecma335;

namespace SoulSmith.Drawing
{
    public class DrawHelpers
    {
        public static IDrawableResource GetDrawableResourceInstance(string key)
        {
            int firstSlash = key.IndexOf('/');

            string type = key.Substring(0, firstSlash);

            IDrawableResource resource = null;

            switch (type.ToLower())
            {
                case "fonts":
                case "simpletext":
                case "simpletextresource":
                    IAssetWrapper<FontResource> wrappedFontResource = AssetManager.Instance.GetFontResource<FontResource>(key);
                    SimpleTextInstance simpleText = new SimpleTextInstance(wrappedFontResource);
                    resource = simpleText;
                    break;
                case "textures":
                case "texture":
                case "texture2d":
                case "texture2dresource":
                case "texture2dinstance":
                    IAssetWrapper<Texture2D> texture = AssetManager.Instance.GetTexture2D<Texture2D>(key);
                    resource = new Texture2DInstance(texture);
                    break;
                case "animations":
                case "animation":
                    IAssetWrapper<Animation.Animation> wrappedAnimation = AssetManager.Instance.GetAnimation<Animation.Animation>(key);
                    AnimationInstance animationPlayer = new AnimationInstance(wrappedAnimation);
                    resource = animationPlayer;
                    break;
                case "zonedtextures":
                case "zonedtexture":
                case "zoned":
                case "zonedtexture2d":
                    resource = AssetManager.Instance.GetZonedResource<ZonedDrawableResource>(key);
                    break;
                default:
                    throw new ArgumentException($"Invalid drawable resource type: {type}");
            }

            if (resource == null)
            {
                throw new InvalidOperationException($"Failed to create drawable resource for key: {key} and type: {type}. Key is invalid or type does not match key.");
            }

            return resource;
        }
    }
}
