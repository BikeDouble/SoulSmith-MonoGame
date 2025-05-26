using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Drawing;
using SoulSmith.Core;

/// <summary>
/// Object to add clipping rectangle to object tree. All children will not be drawn or accept mouse inputs from outside rectangle. Does not support rotation.
/// </summary>
public class ScissorRect : CanvasObject
{
    private int _width;
    private int _height;

    public ScissorRect(
        Position position,
        int width,
        int height,
        DrawableResource sprite = null,
        Dictionary<BoundingZoneType, CanvasObject> boundingZones = null,
        IEnumerable<SoulSmithObject> children = null) : base(position, sprite, boundingZones, children)
    {
        _width = width;
        _height = height;
    }

    public override void CollectDrawPackets(Position absolutePosition, Vector4 tint, IAddOnly<DrawPacket> renderQueue, Rectangle? scissorRect = null)
    {
        Position newPosition = new Position(absolutePosition);
        newPosition.Transform(Position);

        Rectangle newRectangle = GetAbsoluteRect(newPosition);

        if (scissorRect.HasValue)
        {
            newRectangle = Rectangle.Intersect(newRectangle, scissorRect.Value);
        }

        base.CollectDrawPackets(absolutePosition, tint, renderQueue, newRectangle);
    }

    private Rectangle GetAbsoluteRect(IReadOnlyPosition absolutePosition)
    {
        return new Rectangle(absolutePosition.X, absolutePosition.Y, (int)(_width * absolutePosition.Width), (int)(_height * absolutePosition.Height));
    }

    public Rectangle GetRect()
    {
        return new Rectangle(Position.X, Position.Y, (int)(_width*Position.Width), (int)(_height*Position.Height));
    }
}

