
using System;
using System.Collections.Generic;

namespace SoulSmithMoves
{
    public static class MoveTemplateLibrary
    {
        public static Dictionary<string, MoveTemplate> CreateDict(AssetLoader assetLoader)
        {
            List<Dictionary<string, MoveTemplate>> dicts = new List<Dictionary<string, MoveTemplate>>();

            dicts.Add(TypelessMoveTemplates.CreateDict(assetLoader));
            dicts.Add(JoyMoveTemplates.CreateDict(assetLoader));

            return MergeDictionaries(dicts);
        }

        public static Dictionary<string, MoveTemplate> MergeDictionaries(List<Dictionary<string, MoveTemplate>> dicts)
        {
            return DictionaryUtilities.MergeDictionaries<string, MoveTemplate>(dicts);
        }
    }
}