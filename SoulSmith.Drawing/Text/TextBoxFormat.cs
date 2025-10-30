using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Text
{
    [JsonConverter(typeof(TextBoxFormatJsonConverter))]
    public class TextBoxFormat : IDisposable
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int FontSize { get; private set; }
        public int BottomXOffset { get; private set; } // For representing a parallelogram
        public int SpaceBetweenLines { get; private set; }
        public int BorderPadding { get; private set; }
        public int ExtraPaddingForSkewLeft { get { return Math.Max((int)((float)BottomXOffset / Height * FontSize), 0); } }
        public int ExtraPaddingForSkewRight { get { return Math.Max(-(int)((float)BottomXOffset / Height * FontSize), 0); } }
        public int LineWidth { get { return Width - (2 * BorderPadding) - (ExtraPaddingForSkewLeft + ExtraPaddingForSkewRight); } }
        public LinePositioning LinePositioning { get; private set; }


        public TextBoxFormat(int x, int y, int width, int height, int bottomXOffset, int spaceBetweenLines, int borderPadding, int fontSize, LinePositioning linePositioning)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            BottomXOffset = bottomXOffset;
            SpaceBetweenLines = spaceBetweenLines;
            BorderPadding = borderPadding;
            FontSize = fontSize;
            LinePositioning = linePositioning;
        }

        public Rectangle GetBoundsForLine(int lineHeight, int linePadding, int lineNumber)
        {
            int yOffset = lineNumber * (lineHeight + linePadding);

            int xOffsetFromSkew = (int)((float)(yOffset / Height) * BottomXOffset);
            int xOffset = linePadding + xOffsetFromSkew;

            return new Rectangle(X + xOffset, Y + yOffset, Width, lineHeight);
        }

        public List<Vector2> GetLinePositions(int numberOfLines)
        {
            switch (LinePositioning)
            {
                case LinePositioning.Center:
                    return GetCenteredLinePositions(numberOfLines);
                case LinePositioning.Top:
                    return GetTopLinePositions(numberOfLines);
                default:
                    throw new NotImplementedException($"Line positioning {LinePositioning} not implemented");
            }
        }

        private List<Vector2> GetCenteredLinePositions(int numberOfLines)
        {
            List<Vector2> linePositions = new List<Vector2>();
            if (numberOfLines <= 0) return linePositions;

            int totalSpaceTakenByLines = numberOfLines * FontSize + ((numberOfLines - 1) * SpaceBetweenLines);
            int startingYOffset = (Height - totalSpaceTakenByLines) / 2;

            linePositions.Add(new Vector2(X + BorderPadding + ExtraPaddingForSkewLeft + GetXOffset(startingYOffset), Y + startingYOffset));

            for (int i = 1; i < numberOfLines; i++)
            {
                int yOffset = startingYOffset + i * (FontSize + SpaceBetweenLines);
                int xOffsetFromSkew = GetXOffset(yOffset);
                linePositions.Add(new Vector2(X + BorderPadding + ExtraPaddingForSkewLeft + xOffsetFromSkew, Y + yOffset));
            }

            return linePositions;
        }

        private List<Vector2> GetTopLinePositions(int numberOfLines)
        {
            List<Vector2> linePositions = new List<Vector2>();
            if (numberOfLines <= 0) return linePositions;
            linePositions.Add(new Vector2(X + BorderPadding + ExtraPaddingForSkewLeft + GetXOffset(0), Y + BorderPadding));
            for (int i = 1; i < numberOfLines; i++)
            {
                int yOffset = i * (FontSize + SpaceBetweenLines);
                int xOffsetFromSkew = GetXOffset(yOffset);
                linePositions.Add(new Vector2(X + BorderPadding + ExtraPaddingForSkewLeft + xOffsetFromSkew, Y + yOffset + BorderPadding));
            }
            return linePositions;
        }

        private int GetXOffset(int yOffset)
        {
            int yOffsetFromY = yOffset - Y;
            float percentDown = (float)yOffsetFromY / Height;
            return (int)(percentDown * BottomXOffset);
        }

        public Vector2 GetCenter()
        {
            return new Vector2(X + (Width / 2) - GetXOffset(Y + (Height / 2)), Y + (Height / 2));
        }

        public void Dispose() { }
    }
}
