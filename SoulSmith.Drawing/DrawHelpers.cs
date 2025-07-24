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
        public static IDrawableResource GetDrawableResource(DrawableResourceKey key)
        {
            if (key == null) return null;

            return GetDrawableResource(key.Key, key.Type);
        }

        public static IDrawableResource GetDrawableResource(string key, string type)
        {
            switch (type.ToLower())
            {
                case "simpletext":
                case "simpletextresource":
                    IAssetWrapper<FontResource> wrappedFontResource = AssetManager.Instance.GetFontResource<FontResource>(key);
                    SimpleTextInstance simpleText = new SimpleTextInstance(wrappedFontResource);
                    return simpleText;
                case "texture":
                case "texture2d":
                case "texture2dresource":
                case "texture2dinstance":
                    IAssetWrapper<Texture2D> texture = AssetManager.Instance.GetTexture2D<Texture2D>(key);
                    Texture2DInstance resource = new Texture2DInstance(texture);
                    return resource;
                case "animation":
                    IAssetWrapper<Animation.Animation> wrappedAnimation = AssetManager.Instance.GetAnimation<Animation.Animation>(key);
                    AnimationPlayer animationPlayer = new AnimationPlayer(wrappedAnimation);
                    return animationPlayer;
                case "zonedtexture":
                case "zoned":
                case "zonedtexture2d":
                    return AssetManager.Instance.GetZonedResource<ZonedDrawableResource>(key);
                default:
                    throw new ArgumentException($"Invalid drawable resource type: {type}");
            }
        }
    }
}
