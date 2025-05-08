using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class DrawPacket
{
    private IReadOnlyCanvasPosition _position;
    private Vector4 _tint;
    private IDrawableResource _resource;

    public DrawPacket(IReadOnlyCanvasPosition position, Vector4 tint, IDrawableResource resource)
    {
        _position = position;
        _tint = tint;
        _resource = resource;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Vector4 tint = _tint;
        IReadOnlyCanvasPosition position = _position;
        IDrawableResource resourceToDraw = _resource;

        if (resourceToDraw != null)
        {
            resourceToDraw.Draw(position, tint, spriteBatch);
        }
    }

    public int Z { get { return _position.Z; } }
}

