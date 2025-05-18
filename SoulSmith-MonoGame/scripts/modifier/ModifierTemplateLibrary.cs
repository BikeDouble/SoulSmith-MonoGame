
using System;
using System.Collections.Generic;

namespace SoulSmithModifiers;

public static class ModifierTemplateLibrary
{
    public static Dictionary<string, ModifierTemplate> CreateDict()
    {
        List<Dictionary<string, ModifierTemplate>> dicts = new List<Dictionary<string, ModifierTemplate>>();

        dicts.Add(BasicStatModifierTemplates.CreateDict());
        dicts.Add(EffectOnHitModifierTemplates.CreateDict());

        return MergeDictionaries(dicts);
    }

    public static Dictionary<string, ModifierTemplate> MergeDictionaries(List<Dictionary<string, ModifierTemplate>> dicts)
    {
        return DictionaryUtilities.MergeDictionaries<string, ModifierTemplate>(dicts);
    }
}