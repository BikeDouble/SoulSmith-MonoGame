
using System;
using System.Collections.Generic;
using SoulSmith.Core;
using SoulSmith.Collections;

namespace SoulSmith.Battle.Move
{
    public static class MoveTemplateLibrary
    {
        public static Dictionary<string, MoveTemplate> CreateDict()
        {
            List<Dictionary<string, MoveTemplate>> dicts = new List<Dictionary<string, MoveTemplate>>();

            dicts.Add(TypelessMoveTemplates.CreateDict());
            dicts.Add(JoyMoveTemplates.CreateDict());

            return MergeDictionaries(dicts);
        }

        public static Dictionary<string, MoveTemplate> MergeDictionaries(List<Dictionary<string, MoveTemplate>> dicts)
        {
            return DictionaryUtilities.MergeDictionaries(dicts);
        }
    }
}