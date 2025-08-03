using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Input
{
    public class InputManager
    {
        private static bool _mouseLeftClickedLastFrame = false;
        private static bool _mouseRightClickedLastFrame = false;

        public static List<InputType> GetCurrentInputs()
        {
            List<InputType> inputs = new List<InputType> { InputType.MouseHover };

            if (MouseFunctions.IsMouseLeftPressed())
            {
                inputs.Add(InputType.MouseLeftHold);
                if (!_mouseLeftClickedLastFrame)
                {
                    inputs.Add(InputType.MouseLeftClick);
                    _mouseLeftClickedLastFrame = true;
                }
            }
            else
            {
                _mouseLeftClickedLastFrame = false;
            }

            if (MouseFunctions.IsMouseRightPressed())
            {
                inputs.Add(InputType.MouseRightHold);
                if (!_mouseRightClickedLastFrame)
                {
                    inputs.Add(InputType.MouseRightClick);
                    _mouseRightClickedLastFrame = true;
                }
            }
            else
            {
                _mouseRightClickedLastFrame = false;
            }

            return inputs;
        }
    }
}
