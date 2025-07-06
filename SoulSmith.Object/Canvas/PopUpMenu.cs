using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Input;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;

namespace SoulSmith.Object.Canvas;
/// <summary>
/// CanvasItem that hides itself if clicked outside of.
/// </summary>
public class PopUpMenu : CanvasObject
{
    public PopUpMenu() : base() { }

    public PopUpMenu(
        Position position = null,
        IReadOnlyTrackedAsset<ZonedResource> sprite = null,
        IEnumerable<SoulSmithObject> children = null) : base(position, sprite, children)
    { }

    public override void Process(double delta)
    {
        base.Process(delta);
    }
    
    public override void CollectInputPackets(IReadOnlyPosition parentAbsolutePosition, IAddOnly<InputPacket> inputQueue, Position absolutePosition = null)
    {
        Position newPosition = absolutePosition;

        if (newPosition == null)
        {
            newPosition = new Position(parentAbsolutePosition);
            newPosition.Transform(Position);
        }

        if (Visible)
        {
            InputPacket packet = CreateInputPacket(ProcessInput, newPosition, (ZonedResource)Resource, true);
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

