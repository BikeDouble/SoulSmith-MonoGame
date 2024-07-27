
using System;
using System.Collections.Generic;

namespace SoulSmithMoves
{
    public static class MoveTemplateLibrary
    {
        public static Dictionary<string, MoveTemplate> CreateDict()
        {
            List<Dictionary<string, MoveTemplate>> dicts = new List<Dictionary<string, MoveTemplate>>();

            dicts.Add(TypelessMoveTemplates.CreateDict());

            return MergeDictionaries(dicts);
        }

        public static Dictionary<string, MoveTemplate> MergeDictionaries(List<Dictionary<string, MoveTemplate>> dicts)
        {
            return DictionaryUtilities.MergeDictionaries<string, MoveTemplate>(dicts);
        }
    }
}