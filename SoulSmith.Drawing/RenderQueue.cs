using Microsoft.Xna.Framework.Graphics;
using NVorbis.Contracts;
using SoulSmith.Core;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SoulSmith.Drawing
{
    public class RenderQueue : IAddOnly<DrawPacket> //TODO change input in CollectDrawPackets
    {
        public const SpriteSortMode SPRITESORTMODE = SpriteSortMode.Deferred;
        public readonly static SamplerState DEFAULTSAMPLERSTATE = SamplerState.PointClamp;

        private bool _spriteBatchBegan = false;
        private SamplerState _currentSamplerState;
        private List<DrawPacket> _packets = new List<DrawPacket>();
        private RasterizerState _scissorState = new RasterizerState { ScissorTestEnable = true, MultiSampleAntiAlias = true };

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            _spriteBatchBegan = false;
            _currentSamplerState = null;
            
            graphics.ScissorRectangle = graphics.Viewport.Bounds;

            if (_packets.Count == 0) return;

            _packets = _packets.OrderBy((a) => a.Z).ThenBy((a) => a.SamplerState.GetHashCode()).ThenBy((a) => a.ScissorRect.GetHashCode()).ToList();

            foreach (DrawPacket packet in _packets)
            {
                CheckAndUpdateForPacket(spriteBatch, graphics, packet);

                DrawDrawPacket(packet, spriteBatch);
            }

            if (_spriteBatchBegan) spriteBatch.End();
            _spriteBatchBegan = false;
        }

        private void CheckAndUpdateForPacket(SpriteBatch spriteBatch, GraphicsDevice graphics, DrawPacket packet)
        {
            Rectangle currentScissorRect = graphics.ScissorRectangle;

            Rectangle newScissorRect = packet.ScissorRect;
            SamplerState samplerState = packet.SamplerState;

            if ((newScissorRect != currentScissorRect) || (samplerState != _currentSamplerState))
            {
                RestartSpriteBatch(spriteBatch, graphics, newScissorRect, samplerState);
            }
        }

        private void RestartSpriteBatch(SpriteBatch spriteBatch, GraphicsDevice graphics, Rectangle scissorRect, SamplerState samplerState)
        {
            if (scissorRect == graphics.Viewport.Bounds)
            {
                if (_spriteBatchBegan) spriteBatch.End();
                graphics.ScissorRectangle = graphics.Viewport.Bounds;
                spriteBatch.Begin(SPRITESORTMODE, _blendState, samplerState);
                _spriteBatchBegan = true;
            }
            else
            {
                if (_spriteBatchBegan) spriteBatch.End();
                graphics.ScissorRectangle = scissorRect;
                spriteBatch.Begin(SPRITESORTMODE, _blendState, samplerState, rasterizerState: _scissorState);
                _spriteBatchBegan = true;
            }

            _currentSamplerState = samplerState;
        }

        private void DrawDrawPacket(DrawPacket packet, SpriteBatch spriteBatch)
        {
            Color color = packet.Color;
            IReadOnlyPosition position = packet.Position;
            IDrawableResource resourceToDraw = packet.Resource;

            if (resourceToDraw != null)
            {
                resourceToDraw.Draw(position, color, spriteBatch);
            }
        }

        public void Clear()
        {
            _packets.Clear();
        }

        public void Add(DrawPacket packet)
        {
            _packets.Add(packet);
        }

        private BlendState _blendState { get { return BlendState.NonPremultiplied; } }
    }
}