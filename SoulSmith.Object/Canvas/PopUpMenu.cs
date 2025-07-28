using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Drawing.Zoned;
using SoulSmith.Input;
using SoulSmith.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Object.Canvas;
/// <summary>
/// ScissorRect that hides itself if clicked outside of and eats mouse hover events.
/// </summary>
public class PopUpMenu : ScissorRect
{
    public const string POPUPMENUZONEKEY = "menuzone";

    public PopUpMenu(
        Position position = null,
        int width = 0,
        int height = 0,
        ZonedDrawableResource sprite = null,
        IEnumerable<SoulSmithObject> children = null) : base(position, width, height, sprite, children)
    { }

    public override void Process(double delta)
    {
        base.Process(delta);
    }
    
    protected override void CollectInputPacketsInternal(IReadOnlyPosition absolutePosition, IAddOnly<InputPacket> inputQueue)
    {
        if (Visible)
        {
            InputPacket packet = CreateInputPacket(ProcessInput, absolutePosition, (IMultiZone)Resource, POPUPMENUZONEKEY, true);
            inputQueue.Add(packet);
        }

        base.CollectInputPacketsInternal(absolutePosition, inputQueue);
    }

    private InputPacketFuncOutput ProcessInput(InputPacketFuncInput funcInput)
    {
        IReadOnlyList<InputType> inputs = funcInput.Inputs;

        if (!inputs.Contains(InputType.MouseHover))
        {
            if (inputs.Contains(InputType.MouseLeft)) // Clicking outside the menu should not consume the input, but hide the menu.
            {
                Hide();
            }
        }
        else
        {
            InputPacketFuncOutput output = new InputPacketFuncOutput();
            output.ConsumedInputs = new List<InputType>{InputType.MouseHover}; // Consume the mouse hover input to prevent it from propagating further.
            return output;
        }

            return null;
    }
}

