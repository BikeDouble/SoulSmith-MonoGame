using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Drawing.Textures;
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
/// Button that throws event when clicked outside of, does nothing when clicked inside of, but does consume hover and click inputs
/// </summary>
public class InverseButton : CanvasObject
{
    public const string POPUPMENUZONEKEY = "menuzone";

    public InverseButton(
        Position position,
        string spriteKey,
        IEnumerable<SoulSmithObject> children = null) : 
        this(
            position, 
            DrawHelpers.GetDrawableResourceInstance(spriteKey) as ZonedDrawableResourceInstance, 
            null)
    { }

    public InverseButton(
        Position position = null,
        ZonedDrawableResourceInstance sprite = null,
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

    public EventHandler<ButtonPressedEventArgs> ButtonPressedEventHandler;

    private InputPacketFuncOutput ProcessInput(InputPacketFuncArgs args)
    {
        if (args.CapturedInputs.Contains(InputType.MouseHover))
        {
            if (!args.IsMouseHovering)
            {
                if (args.CapturedInputs.Contains(InputType.MouseLeftClick)) // Clicking outside the button should not consume the input.
                {
                    ButtonPressed();
                }
            }
            else // If mouse is hovering this
            {
                InputPacketFuncOutput output = new InputPacketFuncOutput();
                List<InputType> newlyConsumedInputs = new List<InputType> { InputType.MouseHover };
                if (args.IsUsable(InputType.MouseLeftClick)) newlyConsumedInputs.Add(InputType.MouseLeftClick);
                output.NewlyConsumedInputs = newlyConsumedInputs; // If mouse is hovering this, consume the mouse hover input to prevent it from propagating further.
                return output;
            }
        }
        
        return null;
    }

    public void ButtonPressed()
    {
        ButtonPressedEventArgs e = new ButtonPressedEventArgs();

        ButtonPressedEventHandler?.Invoke(this, e);
    }

}

