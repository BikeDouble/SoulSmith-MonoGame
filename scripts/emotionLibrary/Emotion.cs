using Microsoft.Xna.Framework;

namespace SoulSmithEmotions
{
    public class Emotion
    {
        public Color Color { get; private set; }

        public Emotion(Color color)
        {
            Color = color;
        }
    }
}