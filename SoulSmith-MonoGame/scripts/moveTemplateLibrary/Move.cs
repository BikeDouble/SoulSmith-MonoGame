using SoulSmithMoves;
using SoulSmithEmotions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SoulSmithMoves {
    public class Move
    {
        private Emotion _emotion;

        public Move(MoveTemplate template, Emotion emotion, List<Effect> effects)
        {
            FriendlyName = template.FriendlyName;
            Description = template.Description;
            Effects = effects.AsReadOnly();
            TargetingStyle = template.TargetingStyle;
            _emotion = emotion;
        }
    
        public ReadOnlyCollection<Effect> Effects { get; }
        public Emotion Emotion { get { return _emotion; } }
        public MoveTargetingStyle TargetingStyle { get; }
        public string FriendlyName { get; }
        public string Description { get; }
    }
}
