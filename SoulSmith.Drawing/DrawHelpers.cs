using SoulSmith.Asset;
using SoulSmith.Drawing.Animation;
using SoulSmith.Drawing.Text;

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
                    return AssetManager.Instance.GetTexture2DResource<Texture2DResource>(key);
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
