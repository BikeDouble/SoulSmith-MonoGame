using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public interface IDrawPacketGenerator
    {
        public void CollectDrawPackets(Position parentAbsolutePosition, Vector4 tint, IAddOnly<DrawPacket> renderQueue, Microsoft.Xna.Framework.Rectangle? scissorRect = null);
    }
}
