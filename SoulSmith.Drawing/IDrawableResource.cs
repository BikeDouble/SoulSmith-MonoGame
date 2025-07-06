using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Drawing
{
    public interface IDrawableResource : IAsset, IProcessable
    {
        public void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch);
        public void UpdateState(string newState) { }
        public void ChangeSpeed(double speed) { }
        public Vector2 Origin { get; }
        public int Width { get; }
        public int Height { get; }
        public double Speed { get { return 1d; } }
    }
}

