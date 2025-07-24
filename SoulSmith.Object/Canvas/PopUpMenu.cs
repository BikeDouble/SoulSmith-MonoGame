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
/// CanvasItem that hides itself if clicked outside of.
/// </summary>
public class PopUpMenu : CanvasObject
{
    public const string POPUPMENUZONEKEY = "menuzone";

    public PopUpMenu() : base() { }

    public PopUpMenu(
        Position position = null,
        ZonedDrawableResource sprite = null,
        IEnumerable<SoulSmithObject> children = null) : base(position, sprite, children)
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
            if (inputs.Contains(InputType.MouseLeft))
            {
                Hide();
            }
        }

        return null;
    }
}

