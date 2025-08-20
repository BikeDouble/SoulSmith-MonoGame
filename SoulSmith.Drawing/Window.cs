using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Drawing
{
    public class Window
    {
        public const int WINDOWHEIGHT = 900;
        public const int WINDOWWIDTH = 1600;
        public readonly static Rectangle WINDOWRECT = new Rectangle(0, 0, WINDOWWIDTH, WINDOWHEIGHT);
        public const int DISTANCEBETWEENBACKUNITSONEITHERTEAM = WINDOWWIDTH - 2 * 250;
    }
}
