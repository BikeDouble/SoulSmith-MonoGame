using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using SoulSmithMoves;

public class Button : CanvasItem
{
    public event EventHandler<ButtonPressedEventArgs> ButtonPressedEventHandler;

    private bool _hovered = false;

    public Button() : base() { }

    public Button(TrackedResource<ColoredPolygon> resource, Position position = null) : base(resource, position) { }

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
    }

    public virtual void OnMouseExit() 
    {
        _hovered = false;
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
}

public class ButtonPressedEventArgs : EventArgs
{

}