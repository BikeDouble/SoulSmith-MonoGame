
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;

namespace SoulSmith.Drawing.Textures
{
    public class Texture2DInstance : IDrawableResource
    {
        private IAssetWrapper<SoulSmithTexture> _wrappedTexture;

        public SamplerState SamplerState { get { return _wrappedTexture.Value.SamplerState; } }

        public Texture2DInstance(IAssetWrapper<SoulSmithTexture> texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture), "Texture cannot be null.");
            }

            if (texture.Value == null)
            {
                throw new ArgumentException("Wrapped texture must have a valid Texture2D instance.", nameof(texture));
            }

            _wrappedTexture = texture;
        }

        public void Draw(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = GetSpriteEffects(position);
            Vector2 scaleVector = position.ScaleVector;
            if (spriteEffects == SpriteEffects.FlipHorizontally) scaleVector.X *= -1;
            if (spriteEffects == SpriteEffects.FlipVertically) scaleVector.Y *= -1;

            spriteBatch.Draw(
                    Texture.Texture,
                    position.Coordinates,
                    null,
                    color,
                    position.Rotation,
                    Origin,
                    scaleVector,
                    spriteEffects,
                    0f);
        }

        public void DrawSubsection(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch, Rectangle sourceRect, Vector2? subSectionOrigin)
        {
            if (subSectionOrigin == null || !subSectionOrigin.HasValue) subSectionOrigin = Origin;

            SpriteEffects spriteEffects = GetSpriteEffects(position);
            Vector2 scaleVector = position.ScaleVector;
            if (spriteEffects == SpriteEffects.FlipHorizontally) scaleVector.X *= -1;
            if (spriteEffects == SpriteEffects.FlipVertically) scaleVector.Y *= -1;

            spriteBatch.Draw(Texture.Texture, position.Coordinates, sourceRect, color, position.Rotation, subSectionOrigin.Value, scaleVector, spriteEffects, 0f);
        }

        private SpriteEffects GetSpriteEffects(IReadOnlyPosition position)
        {
            if ((position.Width < 0) && (position.Height > 0))
            {
                return SpriteEffects.FlipHorizontally;
            }

            if ((position.Height < 0) && (position.Width > 0))
            {
                return SpriteEffects.FlipVertically;
            }

            return SpriteEffects.None;
        }

        public void Process(double delta) { }

        public void Dispose()
        {
            _wrappedTexture.Dispose();
        }

        private Vector2 GetOriginInternal()
        {
            switch (OriginPlacement)
            {
                case OriginPlacement.TopLeft:
                    return Vector2.Zero;
                case OriginPlacement.Center:
                    return new Vector2(Texture.Width / 2, Texture.Height / 2);
                case OriginPlacement.BottomMiddle:
                    return new Vector2(Texture.Width / 2, Texture.Height);
                case OriginPlacement.TopMiddle:
                    return new Vector2(Texture.Width / 2, 0);
                case OriginPlacement.LeftMiddle:
                    return new Vector2(0, Texture.Height / 2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(OriginPlacement), OriginPlacement, "Invalid origin placement specified.");
            }
        }

        public SoulSmithTexture Texture { get { return _wrappedTexture.Value; } }
        public int Width { get { return Texture.Width; } }
        public int Height { get { return Texture.Height; } }
        public Vector2 Origin { get { return GetOriginInternal(); } }
        public OriginPlacement OriginPlacement { get; set; } = OriginPlacement.Center; 
    }
}