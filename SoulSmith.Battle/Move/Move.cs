using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SoulSmith.EmotionTag;
using SoulSmith.Battle.Effect;

namespace SoulSmith.Battle.Move
{
    public class Move
    {
        private EmotionTag.EmotionTag _emotionTag;

        public Move(MoveTemplate template, EmotionTag.EmotionTag emotion, List<Effect.Effect> effects)
        {
            FriendlyName = template.FriendlyName;
            Description = template.Description;
            Effects = effects.AsReadOnly();
            TargetingStyle = template.TargetingStyle;
            _emotionTag = emotion;
        }

        public ReadOnlyCollection<Effect.Effect> Effects { get; }
        public EmotionTag.EmotionTag EmotionTag { get { return _emotionTag; } }
        public MoveTargetingStyle TargetingStyle { get; }
        public string FriendlyName { get; }
        public string Description { get; }
    }
}
