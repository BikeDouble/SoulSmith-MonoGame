using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public class DrawPacket
    {
        private IReadOnlyPosition _position;
        private Color _color;
        private IDrawableResource _resource;
        private Rectangle? _scissorRect;

        public DrawPacket(IReadOnlyPosition position, Color color, IDrawableResource resource, Rectangle? scissorRect)
        {
            _position = position;
            _color = color;
            _resource = resource;
            _scissorRect = scissorRect;
        }

        public IReadOnlyPosition Position { get { return _position; } }
        public Color Color { get { return _color; } }
        public IDrawableResource Resource { get { return _resource; } }
        public int Z { get { return _position.Z; } }
        public Rectangle? ScissorRect { get { return _scissorRect; } }
    }
}


