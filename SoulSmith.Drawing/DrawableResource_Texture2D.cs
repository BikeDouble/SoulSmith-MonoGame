
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using SoulSmith.Asset;

namespace SoulSmith.Drawing
{
    public class DrawableResource_Texture2D : DrawableResource
    {
        private TrackedAsset<Texture2D> _texture;

        public DrawableResource_Texture2D(DrawableResource_Texture2D other) : base(other)
        {
            _texture = new TrackedAsset<Texture2D>(other._texture);
        }

        public DrawableResource_Texture2D(TrackedAsset<Texture2D> texture)
        {
            _texture = texture;
        }

        public override object DeepClone()
        {
            return new DrawableResource_Texture2D(this);
        }

        public override void Draw(IReadOnlyCanvasPosition position, Vector4 tint, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                    _texture.Resource,
                    position.Coordinates,
                    null,
                    GetTintedColor(tint),
                    position.Rotation,
                    new Vector2(0, 0),
                    position.ScaleVector,
                    SpriteEffects.None,
                    0f);
        }
    }
}