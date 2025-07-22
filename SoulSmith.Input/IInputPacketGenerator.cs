using SoulSmith.Core;

namespace SoulSmith.Input
{
    public interface IInputPacketGenerator
    {
        public void CollectInputPackets(IReadOnlyPosition parentAbsolutePosition, IAddOnly<InputPacket> inputQueue);
    }
}
