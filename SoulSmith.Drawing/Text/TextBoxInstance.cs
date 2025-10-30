using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Drawing.Animation;
using SoulSmith.Drawing.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Text
{
    public class TextBoxInstance : IDrawableTextResource
    {
        private IAssetWrapper<TextBox> _wrappedTextBox;
        private string _text = string.Empty;
        private List<TextLine> _lines = new List<TextLine>();
        private List<Vector2> _linePositions = new List<Vector2>();

        public SamplerState SamplerState { get { return SamplerState.LinearClamp; } }

        internal TextBoxInstance(IAssetWrapper<TextBox> textBox)
        {
            _wrappedTextBox = textBox;
        }

        public void Draw(IReadOnlyPosition postion, Color color, SpriteBatch spriteBatch) 
        {
            if (_wrappedTextBox == null) throw new ArgumentNullException(nameof(_wrappedTextBox));

            if (_wrappedTextBox.Value == null) throw new ArgumentNullException(nameof(_wrappedTextBox.Value));

            Vector2 origin = Origin;

            List<Vector2> transformedLinePositions = _linePositions.Select(lp => new Vector2((lp.X - origin.X) * postion.ScaleVector.X, (lp.Y - origin.Y) * postion.ScaleVector.Y)).ToList();

            for (int i = 0; i < _lines.Count; i++)
            {
                Position linePosition = new Position(postion);
                linePosition.Translate(transformedLinePositions[i]);
                linePosition.Scale(new Vector2((float)_wrappedTextBox.Value.Format.FontSize / FontResource.STANDARDFONTSIZE));
                _lines[i].DrawLine(linePosition, _wrappedTextBox.Value.Font, color, spriteBatch);
            }
        }

        public void UpdateState(string newState, bool force = false) 
        { 
            _text = newState;
            _lines = SplitTextIntoLeftAlignedLines(_text, _wrappedTextBox.Value.Font, _wrappedTextBox.Value.Format.LineWidth, _wrappedTextBox.Value.Format.FontSize);
            _linePositions = _wrappedTextBox.Value.Format.GetLinePositions(_lines.Count);
        }

        public void Dispose()
        {
            _wrappedTextBox?.Dispose();
        }

        internal static List<TextLine> SplitTextIntoLeftAlignedLines(string text, IFontResource font, int lineLength, int fontSize)
        {
            var lines = new List<TextLine>();
            if (string.IsNullOrEmpty(text) || font == null || lineLength <= 0)
                return lines;

            int spaceWidth = (int)font.MeasureString(" ", fontSize).X;
            string[] words = text.Split(' ');
            StringBuilder currentLine = new StringBuilder();
            int currentLineWidth = 0;
            int wordCount = 0;

            foreach (var word in words)
            {
                string wordToAdd = wordCount > 0 ? " " + word : word;
                int wordWidth = (int)font.MeasureString(wordToAdd, fontSize).X;

                if (currentLineWidth + wordWidth > lineLength && currentLine.Length > 0)
                {
                    // Add current line
                    string lineText = currentLine.ToString();
                    float wordWidthMultiplier = 1;
                    lines.Add(new TextLine(lineText, wordWidthMultiplier, spaceWidth));
                    currentLine.Clear();
                    currentLineWidth = 0;
                    wordCount = 0;
                    wordToAdd = word; // Don't add space for first word in new line
                    wordWidth = (int)font.MeasureString(wordToAdd, fontSize).X;
                }

                currentLine.Append(wordToAdd);
                currentLineWidth += wordWidth;
                wordCount++;
            }

            if (currentLine.Length > 0)
            {
                string lineText = currentLine.ToString();
                float wordWidthMultiplier = 1;
                lines.Add(new TextLine(lineText, wordWidthMultiplier, spaceWidth));
            }

            return lines;
        }

        //private static int CalculateRequiredLines(string text, IFontResource font, int lineLength)
        //{
        //    if (string.IsNullOrEmpty(text)) return 0;
            
        //    int spaceWidth = (int)font.MeasureString(" ").X;

        //    int totalLength = (int)font.MeasureString(text).X;

        //    int requiredLines = (int)Math.Ceiling((float)totalLength / lineLength);

        //    //Check if by removing spaces at end of lines we can fit more text
        //    int lengthSavedByRemovingSpaces = spaceWidth * (requiredLines - 2);
        //    int requiredLinesWithSpacesRemoved = (int)Math.Ceiling((float)(totalLength - lengthSavedByRemovingSpaces) / lineLength);

        //    return Math.Min(requiredLines, requiredLinesWithSpacesRemoved);
        //}

        private Vector2 GetOriginInternal()
        {
            switch (OriginPlacement)
            {
                case OriginPlacement.TopLeft:
                    return Vector2.Zero;
                case OriginPlacement.Center:
                    return _wrappedTextBox.Value.Format.GetCenter();
                case OriginPlacement.TopMiddle:
                    return new Vector2(_wrappedTextBox.Value.Format.Width / 2 + Math.Abs(_wrappedTextBox.Value.Format.BottomXOffset / 2), 0);
                case OriginPlacement.LeftMiddle:
                    return new Vector2(0, _wrappedTextBox.Value.Format.Height / 2 + _wrappedTextBox.Value.Format.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(OriginPlacement), OriginPlacement, "Invalid origin placement specified.");
            }
        }

        public void Process(double delta) { }

        public Vector2 Origin { get { return GetOriginInternal(); } }
        public OriginPlacement OriginPlacement { get; set; } = OriginPlacement.Center;
        public int Width { get { return (int)_wrappedTextBox.Value.Format.Width + _wrappedTextBox.Value.Format.BottomXOffset; } }
        public int Height { get { return (int)_wrappedTextBox.Value.Format.Height; } }
    }
}
