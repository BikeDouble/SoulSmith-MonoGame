using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmithInput;
using SoulSmith.Drawing;
using SoulSmith.Core;

/// <summary>
/// CanvasItem that hides itself if clicked outside of.
/// </summary>
public class PopUpMenu : CanvasObject
{
    public PopUpMenu() : base() { }

    public PopUpMenu(
        Position position = null,
        DrawableResource sprite = null,
        Dictionary<BoundingZoneType, CanvasObject> boundingZones = null,
        IEnumerable<SoulSmithObject> children = null) : base(position, sprite, boundingZones, children)
    {}

    public override void Process(double delta)
    {
        base.Process(delta);
    }

    public override void CollectInputPackets(Position parentAbsolutePosition, IAddOnly<InputPacket> inputQueue, Position absolutePosition = null)
    {
        Position newPosition = absolutePosition;

        if (newPosition == null)
        {
            newPosition = new Position(parentAbsolutePosition);
            newPosition.Transform(Position);
        }

        if (Visible)
        {
            InputPacket packet = CreateInputPacket(ProcessInput, newPosition, true);
            inputQueue.Add(packet);
        }

        base.CollectInputPackets(newPosition, inputQueue);
    }

    private InputPacketFuncOutput ProcessInput(InputPacketFuncInput funcInput)
    {
        IReadOnlyList<InputType> inputs = funcInput.Inputs;

        if (!inputs.Contains(InputType.MouseHover))
        {
            if (inputs.Contains(InputType.MouseLeft))
            {
                Hide();
            }
        }

        return null;
    }
}

