using SoulSmith.Input;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;

namespace SoulSmith.Object.Canvas;
public class ButtonObject : CanvasObject_MultipleResources
{
    public event EventHandler<ButtonPressedEventArgs> ButtonPressedEventHandler;
    private bool _hovered = false;
    private int _idleResourceIndex = -1;
    private int _hoveredResourceIndex = -1;

    public ButtonObject(
        IAssetWrapper<ZonedResource> idleResource,
        IAssetWrapper<ZonedResource> hoveredResource,
        Position position = null) : base(idleResource, hoveredResource, position)
    {
        _idleResourceIndex = 0;
        _hoveredResourceIndex = 1;
    }

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

        if (IsVisible())
        {
            InputPacket packet = CreateInputPacket(ProcessInputs, newPosition, (ZonedResource)Resource, true);
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
            consumedInputs = new List<InputType> { InputType.MouseHover };

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

    protected ZonedResource IdleResource { get { return GetResource(_idleResourceIndex) as ZonedResource; } }
    protected ZonedResource HoveredResource { get { return GetResource(_hoveredResourceIndex) as ZonedResource; } }
}

public class ButtonPressedEventArgs : EventArgs
{

}