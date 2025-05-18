using Microsoft.Xna.Framework;
using SoulSmithModifiers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SoulSmithEmotions
{
    public class Emotion
    {
        public Color Color { get; }
        public ReadOnlyCollection<ModifierTemplateWithArgs> PermanentModifiers { get; }

        public Emotion(Color color, List<ModifierTemplateWithArgs> permanentModifiers = null)
        {
            Color = color;

            PermanentModifiers = permanentModifiers?.AsReadOnly();
        }
    }
}