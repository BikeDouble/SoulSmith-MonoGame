
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public class Texture2DResource : IDrawableResource
    {
        private Texture2D _texture;

        public Texture2DResource(Texture2D texture)
        {
            _texture = texture;
        }

        public void Draw(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                    _texture,
                    position.Coordinates,
                    null,
                    color,
                    position.Rotation,
                    Origin,
                    position.ScaleVector,
                    SpriteEffects.None,
                    0f);
        }

        public void DrawSubsection(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch, Rectangle sourceRect, Vector2? subSectionOrigin)
        {
            if ((subSectionOrigin == null) || (!subSectionOrigin.HasValue)) subSectionOrigin = Origin; 

            spriteBatch.Draw(_texture, position.Coordinates, sourceRect, color, position.Rotation, subSectionOrigin.Value, position.ScaleVector, SpriteEffects.None, 0f);
        }

        public void Process(double delta) { }

        public void Dispose()
        {
            _texture.Dispose();
        }

        public int Width { get { return _texture.Width; } }
        public int Height { get { return _texture.Height; } }
        public Vector2 Origin { get { return new Vector2(_texture.Width / 2, _texture.Height / 2); } }
    }
}