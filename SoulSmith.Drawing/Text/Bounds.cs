using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Text
{
    internal class Bounds
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BottomXOffset { get; set; } // For representing a parallelogram

        public Bounds(int x, int y, int width, int height, int bottomXOffset)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            BottomXOffset = bottomXOffset;
        }

        public Rectangle GetBoundsForLine(int lineHeight, int linePadding, int lineNumber)
        {
            int yOffset = lineNumber * (lineHeight + linePadding);

            int xOffsetFromSkew = (int)((float)(yOffset / Height) * BottomXOffset);
            int xOffset = linePadding + xOffsetFromSkew;

            return new Rectangle(X + xOffset, Y + yOffset, Width, lineHeight);
        }
    }
}
