using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using SoulSmithMoves;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using SoulSmithInput;

public class Button : CanvasItem_MultipleResources
{
    public event EventHandler<ButtonPressedEventArgs> ButtonPressedEventHandler;
    private bool _hovered = false;
    private int _idleResourceIndex = -1;
    private int _hoveredResourceIndex = -1;

    public Button() : base() { }

    public Button(
        DrawableResource resource,
        DrawableResource hoveredResource,
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = null,
        CanvasPosition position = null) : base(resource, hoveredResource, boundingZones, position) 
    {
        _idleResourceIndex = 0;
        _hoveredResourceIndex = 1;
    }

    public override void Process(double delta)
    {
        base.Process(delta);
    }

    public override void CollectInputPackets(CanvasPosition parentAbsolutePosition, IAddOnly<InputPacket> inputQueue, CanvasPosition absolutePosition = null)
    {
        CanvasPosition newPosition = absolutePosition; 

        if (newPosition == null)
        {
            newPosition = new CanvasPosition(parentAbsolutePosition);
            newPosition.Transform(Position);
        }

        if (IsVisible())
        {
            InputPacket packet = CreateInputPacket(ProcessInputs, newPosition, true);
            inputQueue.Add(packet);
        }

        base.CollectInputPackets(newPosition, inputQueue);
    }

    private InputPacketFuncOutput ProcessInputs(InputPacketFuncInput funcInput)
    {
        IReadOnlyList<InputType> inputTypes = funcInput.Inputs;

        List<InputType> consumedInputs = null;

        if (inputTypes.Contains(InputType.MouseHover))
        {
            consumedInputs = new List<InputType> { InputType.MouseHover};

            if (!_hovered)
                OnMouseEnter();

            if (inputTypes.Contains(InputType.MouseLeft))
            {
                ButtonPressed();
                consumedInputs.Add(InputType.MouseLeft);
            }
        }
        else
        {
            if (_hovered)
                OnMouseExit();
        }

        InputPacketFuncOutput output = new InputPacketFuncOutput();
        output.ConsumedInputs = consumedInputs;

        return output;
    }

    public virtual void OnMouseEnter()
    {
        _hovered = true;

        if (_hoveredResourceIndex >= 0)
            SetActiveResourceIndex(_hoveredResourceIndex);
    }

    public virtual void OnMouseExit() 
    {
        _hovered = false;

        SetActiveResourceIndex(_idleResourceIndex);
    }

    public void ButtonPressed()
    {
        ButtonPressedEventArgs e = new ButtonPressedEventArgs();

        ButtonPressedEventHandler?.Invoke(this, e);
    }

    protected DrawableResource IdleResource { get { return GetResource(_idleResourceIndex); } }
    protected DrawableResource HoveredResource {  get { return GetResource(_hoveredResourceIndex); } }
}

public class ButtonPressedEventArgs : EventArgs
{

}