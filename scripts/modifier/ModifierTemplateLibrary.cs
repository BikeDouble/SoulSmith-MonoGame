
using System;
using System.Collections.Generic;

namespace SoulSmithModifiers;

public static class ModifierTemplateLibrary
{
    public static Dictionary<string, ModifierTemplate> CreateDict(AssetLoader assetLoader)
    {
        List<Dictionary<string, ModifierTemplate>> dicts = new List<Dictionary<string, ModifierTemplate>>();

        dicts.Add(BasicStatModifierTemplates.CreateDict(assetLoader));
        dicts.Add(EffectOnHitModifierTemplates.CreateDict(assetLoader));

        return MergeDictionaries(dicts);
    }

    public static Dictionary<string, ModifierTemplate> MergeDictionaries(List<Dictionary<string, ModifierTemplate>> dicts)
    {
        return DictionaryUtilities.MergeDictionaries<string, ModifierTemplate>(dicts);
    }
}