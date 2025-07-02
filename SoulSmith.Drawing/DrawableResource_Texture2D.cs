
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
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
                    Origin,
                    position.ScaleVector,
                    SpriteEffects.None,
                    0f);
        }

        public void Dispose()
        {
            _texture.Dispose();
        }

        public int Width { get { return _texture.Width; } }
        public int Height { get { return _texture.Height; } }
        public Vector2 Origin { get { return new Vector2(_texture.Width / 2, _texture.Height / 2); } }
    }
}