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
        public static IAssetWrapper<IDrawableResource> GetDrawableResource(DrawableResourceKey key)
        {
            if (key == null) return null;

            return GetDrawableResource(key.Key, key.Type);
        }

        public static IAssetWrapper<IDrawableResource> GetDrawableResource(string key, string type)
        {
            switch (type.ToLower())
            {
                case "simpletext":
                case "simpletextresource":
                    IAssetWrapper<FontResource> wrappedFontResource = AssetManager.Instance.GetFontResource<FontResource>(key);
                    IAssetWrapper<SimpleTextInstance> wrappedSimpleText = new UntrackedAssetWrapper<SimpleTextInstance>(new SimpleTextInstance(wrappedFontResource));
                    return wrappedSimpleText;
                case "texture":
                case "texture2d":
                case "texture2dresource":
                    IAssetWrapper<Texture2D> texture = AssetManager.Instance.GetTexture2D<Texture2D>(key);
                    Texture2DResource resource = new Texture2DResource(texture);
                    return new UntrackedAssetWrapper<Texture2DResource>(resource);
                case "animation":
                    IAssetWrapper<Animation.Animation> wrappedAnimation = AssetManager.Instance.GetAnimation<Animation.Animation>(key);
                    IAssetWrapper<AnimationPlayer> wrappedAnimationPlayer = new UntrackedAssetWrapper<AnimationPlayer>(new AnimationPlayer(wrappedAnimation));
                    return wrappedAnimationPlayer;
                case "zonedtexture":
                case "zoned":
                case "zonedtexture2d":
                    return AssetManager.Instance.GetZonedResource<ZonedResource>(key);
                default:
                    throw new ArgumentException($"Invalid drawable resource type: {type}");
            }
        }
    }
}
