using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Drawing;
using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Shapes;

namespace SoulSmith.Object.Canvas;
/// <summary>
/// Object to add clipping rectangle to draw tree. All children will not be drawn or accept mouse inputs from outside rectangle. Does not support rotation.
/// </summary>
public class ScissorRect : CanvasObject
{
    public int Width { get; set; }
    public int Height { get; set; }
    public OriginPlacement RectOriginPlacement { get; private set; }

    public ScissorRect(
        Position position,
        int width,
        int height,
        IDrawableResource sprite = null,
        IEnumerable<SoulSmithObject> children = null) : base(position, sprite, children)
    {
        Width = width;
        Height = height;
    }

    public override void CollectDrawPackets(IReadOnlyPosition absolutePosition, Color color, IAddOnly<DrawPacket> renderQueue, Rectangle? scissorRect = null)
    {
        Position newPosition = new Position(absolutePosition);
        newPosition.TransformInContext(Position, absolutePosition);

        Rectangle newRectangle = GetAbsoluteRect(newPosition);

        if (scissorRect.HasValue)
        {
            newRectangle = Rectangle.Intersect(newRectangle, scissorRect.Value);
        }

        base.CollectDrawPackets(absolutePosition, color, renderQueue, newRectangle);
    }

    private Rectangle GetAbsoluteRect(IReadOnlyPosition absolutePosition) //TODO implement 90 degree rotation?
    {
        int absRectX = 0;
        int absRectY = 0;
        int absWidth = (int)(Width * absolutePosition.Width);
        int absHeight = (int)(Height * absolutePosition.Height);

        switch (RectOriginPlacement)
        {
            case OriginPlacement.Center:
                absRectX = -(absWidth / 2);
                absRectY = -(absHeight / 2);
                break;
            case OriginPlacement.TopLeft:
                break;
            case OriginPlacement.TopMiddle:
                absRectX = -(absWidth / 2);
                break;
            case OriginPlacement.BottomMiddle:
                absRectX = -(absWidth / 2);
                absRectY = -absHeight;
                break;
            default:
                throw new NotImplementedException($"Origin placement {RectOriginPlacement.ToString()} not implemented for ScissorRect.");
        }

        
        return GetNonNegativeRect(absRectX + (int)absolutePosition.X, absRectY + (int)absolutePosition.Y, absWidth, absHeight);
    }
    
    private Rectangle GetNonNegativeRect(int x, int y, int width, int height)
    {
        if (width >= 0 && height >= 0) // Both are positive
        {
            return new Rectangle(x, y, width, height);
        }
        else if (width >= 0 && height < 0) // Only height is negative
        {
            height = -height;
            return new Rectangle(x, y - height, width, height);
        }
        else if (width < 0 && height >= 0) // Only width is negative
        {
            width = -width;
            return new Rectangle(x - width, y, width, height);
        }
        else // Both are negative
        {
            width = -width;
            height = -height;
            return new Rectangle(x - width, y - height, width, height);
        }
    }

    public override void SetOriginPlacement(OriginPlacement originPlacement)
    {
        base.SetOriginPlacement(originPlacement);

        RectOriginPlacement = originPlacement;
    }
}

