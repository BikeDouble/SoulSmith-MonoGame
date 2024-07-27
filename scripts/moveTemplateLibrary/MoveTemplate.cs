using SoulSmithMoves;
using SoulSmithEmotions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace SoulSmithMoves
{
    public class MoveTemplate
    {
        public MoveTemplate(string name,
                        string description,
                        List<Effect> effects,
                        MoveTargetingStyle targetingStyle = MoveTargetingStyle.Enemy,
                        EmotionTag type = EmotionTag.Typeless)
        {
            FriendlyName = name;
            Description = description;
            Effects = effects.AsReadOnly();
            TargetingStyle = targetingStyle;
            EmotionTag = type;
        }

        public ReadOnlyCollection<Effect> Effects { get; }
        public EmotionTag EmotionTag { get; }
        public MoveTargetingStyle TargetingStyle { get; }
        public string FriendlyName { get; }
        public string Description { get; }
    }
}
