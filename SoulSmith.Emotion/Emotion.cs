using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmith.Battle.Modifier;

namespace SoulSmith.Emotion
{
    public class Emotion
    {
        public Color Color { get; }
        public ReadOnlyCollection<IModifier> PermanentModifiers { get; }

        public Emotion(Color color, List<IModifier> permanentModifiers = null)
        {
            Color = color;

            PermanentModifiers = permanentModifiers?.AsReadOnly();
        }
    }
}