using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Battle.Effect;
using SoulSmith.EmotionTag;

namespace SoulSmith.Battle.Move
{
    public class MoveTemplate
    {
        public MoveTemplate(string name,
                        string description,
                        IEnumerable<EffectTemplate> effects,
                        MoveTargetingStyle targetingStyle = MoveTargetingStyle.Enemy,
                        EmotionTag.EmotionTag type = SoulSmith.EmotionTag.EmotionTag.Typeless)
        {
            FriendlyName = name;
            Description = description;
            Effects = effects?.ToList().AsReadOnly();
            TargetingStyle = targetingStyle;
            EmotionTag = type;
        }

        public ReadOnlyCollection<EffectTemplate> Effects { get; }
        public EmotionTag.EmotionTag EmotionTag { get; }
        public MoveTargetingStyle TargetingStyle { get; }
        public string FriendlyName { get; }
        public string Description { get; }
    }
}
