using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public interface IDrawableResource : IAsset
    {
        public void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch);
        public void UpdateState(string newState) { }
    }
}

