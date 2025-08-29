using SoulSmith.Input;
using SoulSmith.Core;
using SoulSmith.Asset;
using SoulSmith.Shapes;
using SoulSmith.Drawing.Zoned;
using SoulSmith.Drawing;

namespace SoulSmith.Object.Canvas;
public class ButtonObject : CanvasObject_MultipleResources
{
    public const string CLICKZONEKEY = "clickzone";
    public event EventHandler<ButtonPressedEventArgs> ButtonPressedEventHandler;
    private bool _hovered = false;
    private int _idleResourceIndex = -1;
    private int _hoveredResourceIndex = -1;
    private float _timeHovered = 0f;
    private float _timeUnhovered = 0f;

    public ButtonObject(
        string idleResourceKey,
        string hoveredResourceKey,
        Position position = null) : 
        this(
            DrawHelpers.GetDrawableResourceInstance(idleResourceKey) as ZonedDrawableResourceInstance,
            DrawHelpers.GetDrawableResourceInstance(hoveredResourceKey) as ZonedDrawableResourceInstance, 
            position)
    { }

    public ButtonObject(
        ZonedDrawableResourceInstance idleResource,
        ZonedDrawableResourceInstance hoveredResource,
        Position position = null) : base(idleResource, hoveredResource, position)
    {
        _idleResourceIndex = 0;
        _hoveredResourceIndex = 1;
    }

    public override void Process(double delta)
    {
        base.Process(delta);

        if (_hovered)
        {
            _timeHovered += (float)delta;
            _timeUnhovered = 0f;
        }
        else
        {
            _timeUnhovered += (float)delta;
            _timeHovered = 0f;
        }
    }

    protected override void CollectInputPacketsInternal(IReadOnlyPosition absolutePosition, IAddOnly<InputPacket> inputQueue)
    {
        if (IsVisible())
        {
            InputPacket packet = CreateInputPacket(ProcessInputs, absolutePosition, (IMultiZone)Resource, CLICKZONEKEY, true);
            inputQueue.Add(packet);
        }

        base.CollectInputPacketsInternal(absolutePosition, inputQueue);
    }

    private InputPacketFuncOutput ProcessInputs(InputPacketFuncArgs input)
    {
        List<InputType> newlyConsumedInputs = null;

        if (input.IsUsable(InputType.MouseHover))
        {
            newlyConsumedInputs = new List<InputType> { InputType.MouseHover };

            if (!_hovered)
                OnMouseEnter();

            if (input.IsUsable(InputType.MouseLeftClick))
            {
                ButtonPressed();
                newlyConsumedInputs.Add(InputType.MouseLeftClick);
            }
        }
        else
        {
            if (_hovered)
                OnMouseExit();
        }

        InputPacketFuncOutput output = new InputPacketFuncOutput();
        output.NewlyConsumedInputs = newlyConsumedInputs;

        return output;
    }

    public virtual void OnMouseEnter()
    {
        _hovered = true;
        _timeUnhovered = 0f;
        _timeHovered = 0f;

        if (_hoveredResourceIndex >= 0)
            SetActiveResourceIndex(_hoveredResourceIndex);
    }

    public virtual void OnMouseExit()
    {
        _hovered = false;
        _timeHovered = 0f;
        _timeUnhovered = 0f;

        SetActiveResourceIndex(_idleResourceIndex);
    }

    public void ButtonPressed()
    {
        ButtonPressedEventArgs e = new ButtonPressedEventArgs();

        ButtonPressedEventHandler?.Invoke(this, e);
    }

    public bool IsHovered { get { return _hovered; } }
    public float TimeHovered { get { return _timeHovered; } }
    public float TimeUnhovered { get { return _timeUnhovered; } }
    protected ZonedDrawableResourceInstance IdleResource { get { return GetResource(_idleResourceIndex) as ZonedDrawableResourceInstance; } }
    protected ZonedDrawableResourceInstance HoveredResource { get { return GetResource(_hoveredResourceIndex) as ZonedDrawableResourceInstance; } }
}

public class ButtonPressedEventArgs : EventArgs
{

}