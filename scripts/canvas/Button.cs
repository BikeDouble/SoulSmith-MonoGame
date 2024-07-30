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
        Position position = null) : base(resource, hoveredResource, boundingZones, position) 
    {
        _idleResourceIndex = 0;
        _hoveredResourceIndex = 1;
    }

    public override void Process(double delta)
    {
        if (IsVisible())
        {
            if (IsMouseOver())
            {
                if (!_hovered)
                    OnMouseEnter();

                if (IsMouseLeftPressed())
                {
                    ButtonPressed();
                }
            }
            else
            {
                if (_hovered)
                    OnMouseExit();
            }
        }

        base.Process(delta);
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

    public bool IsMouseOver()
    {
        if (Resource is null)
            return false;

        Position globalPosition = GetGlobalPosition();
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
        Vector2 localMousePosition = mousePosition - globalPosition.Coordinates;

        return Resource.ContainsPoint(localMousePosition, Position);
    }

    public static bool IsMouseLeftPressed()
    {
        MouseState mouseState = Mouse.GetState();
        return (mouseState.LeftButton == ButtonState.Pressed);
    }

    protected DrawableResource IdleResource { get { return GetResource(_idleResourceIndex); } }
    protected DrawableResource HoveredResource {  get { return GetResource(_hoveredResourceIndex); } }
}

public class ButtonPressedEventArgs : EventArgs
{

}