using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SoulSmithInput;

public static class MouseFunctions
{

    

    public static Vector2 GetPosition()
    {
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
        return mousePosition;
    }

    public static Vector2 GetRelativePosition(IReadOnlyCanvasPosition objectPosition)
    {
        Vector2 objectCoords = objectPosition.Coordinates;
        return GetPosition() - objectCoords;
    }

    public static bool IsMouseLeftPressed()
    {
        MouseState mouseState = Mouse.GetState();
        return (mouseState.LeftButton == ButtonState.Pressed);
    }

    public static bool IsMouseRightPressed()
    {
        MouseState mouseState = Mouse.GetState();
        return (mouseState.RightButton == ButtonState.Pressed);
    }
}
