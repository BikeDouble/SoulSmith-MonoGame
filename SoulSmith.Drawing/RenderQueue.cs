using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using SoulSmith.Core;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SoulSmith.Drawing
{
    public class RenderQueue : IAddOnly<DrawPacket> //TODO change input in CollectDrawPackets
    {
        private bool drawScissorRect = true;
        private List<DrawPacket> _packets = new List<DrawPacket>();
        private RasterizerState _scissorState = new RasterizerState { ScissorTestEnable = true };

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            graphics.ScissorRectangle = graphics.Viewport.Bounds;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            if (_packets.Count == 0) return;

            _packets.Sort((a, b) => a.Z.CompareTo(b.Z));

            foreach (DrawPacket packet in _packets)
            {
                CheckAndUpdateScissorRect(spriteBatch, graphics, packet.ScissorRect);

                if (drawScissorRect) DrawScissorRect(graphics.ScissorRectangle, spriteBatch);

                DrawDrawPacket(packet, spriteBatch);
            }

            spriteBatch.End();
        }

        private void DrawScissorRect(Rectangle rect, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(rect, Microsoft.Xna.Framework.Color.White);
        }

        private void CheckAndUpdateScissorRect(SpriteBatch spriteBatch, GraphicsDevice graphics, Rectangle? newScissorRect)
        {
            Rectangle currentScissorRect = graphics.ScissorRectangle;

            if (newScissorRect.HasValue)
            {
                if (currentScissorRect != newScissorRect.Value)
                {
                    UpdateSpriteBatchScissorRect(spriteBatch, graphics, newScissorRect.Value);
                }
            }
            else if (currentScissorRect != graphics.Viewport.Bounds)
            {
                UpdateSpriteBatchScissorRect(spriteBatch, graphics, graphics.Viewport.Bounds);
            }
        }

        private void UpdateSpriteBatchScissorRect(SpriteBatch spriteBatch, GraphicsDevice graphics, Rectangle scissorRect)
        {
            if (scissorRect == graphics.Viewport.Bounds)
            {
                spriteBatch.End();
                graphics.ScissorRectangle = graphics.Viewport.Bounds;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            }
            else
            {
                spriteBatch.End();
                graphics.ScissorRectangle = scissorRect;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, rasterizerState: _scissorState);
            }
        }

        private void DrawDrawPacket(DrawPacket packet, SpriteBatch spriteBatch)
        {
            Vector4 tint = packet.Tint;
            IReadOnlyCanvasPosition position = packet.Position;
            IDrawableResource resourceToDraw = packet.Resource;

            if (resourceToDraw != null)
            {
                resourceToDraw.Draw(position, tint, spriteBatch);
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
    }
}