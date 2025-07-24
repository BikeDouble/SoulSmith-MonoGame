
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;

namespace SoulSmith.Drawing.Textures
{
    public class Texture2DResource : IDrawableResource
    {
        private IAssetWrapper<Texture2D> _wrappedTexture;

        public Texture2DResource(IAssetWrapper<Texture2D> texture)
        {
            _wrappedTexture = texture;
        }

        public void Draw(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                    Texture,
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
            if (subSectionOrigin == null || !subSectionOrigin.HasValue) subSectionOrigin = Origin; 

            spriteBatch.Draw(Texture, position.Coordinates, sourceRect, color, position.Rotation, subSectionOrigin.Value, position.ScaleVector, SpriteEffects.None, 0f);
        }

        public void Process(double delta) { }

        public void Dispose()
        {
            _wrappedTexture.Dispose();
        }

        public Texture2D Texture { get { return _wrappedTexture.Value; } }
        public int Width { get { return Texture.Width; } }
        public int Height { get { return Texture.Height; } }
        public Vector2 Origin { get { return new Vector2(Texture.Width / 2, Texture.Height / 2); } }
        public OriginPlacement OriginPlacement { get; set; } = OriginPlacement.Center; //TODO: Make this configurable
    }
}