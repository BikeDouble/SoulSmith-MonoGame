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
        private Vector4 _tint;
        private IDrawableResource _resource;
        private Rectangle? _scissorRect;

        public DrawPacket(IReadOnlyPosition position, Vector4 tint, IDrawableResource resource, Rectangle? scissorRect)
        {
            _position = position;
            _tint = tint;
            _resource = resource;
            _scissorRect = scissorRect;
        }

        public IReadOnlyPosition Position { get { return _position; } }
        public Vector4 Tint { get { return _tint; } }
        public IDrawableResource Resource { get { return _resource; } }
        public int Z { get { return _position.Z; } }
        public Rectangle? ScissorRect { get { return _scissorRect; } }
    }
}


