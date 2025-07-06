using SoulSmith.Asset;
using SoulSmith.Drawing.Animation;

namespace SoulSmith.Drawing
{
    public class DrawHelpers
    {
        public static IAssetWrapper<IDrawableResource> GetDrawableResource(string key, string type)
        {
            switch (type.ToLower())
            {
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
