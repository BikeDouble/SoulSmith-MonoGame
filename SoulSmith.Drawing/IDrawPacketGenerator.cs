using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public interface IDrawPacketGenerator
    {
        public void CollectDrawPackets(IReadOnlyPosition parentAbsolutePosition, Color color, IAddOnly<DrawPacket> renderQueue, Microsoft.Xna.Framework.Rectangle? scissorRect = null);
    }
}
