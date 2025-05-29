
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public class DrawableResource_Texture2D : IDrawableResource
    {
        private Texture2D _texture;

        public DrawableResource_Texture2D(Texture2D texture)
        {
            _texture = texture;
        }

        public void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                    _texture,
                    position.Coordinates,
                    null,
                    new Color(tint),
                    position.Rotation,
                    new Vector2(0, 0),
                    position.ScaleVector,
                    SpriteEffects.None,
                    0f);
        }

        public void Dispose()
        {
            _texture.Dispose();
        }
    }
}