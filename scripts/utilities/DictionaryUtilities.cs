
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;

// Base class for data storage with creation at runtime
public static class DictionaryUtilities
{ 
    public static Dictionary<Tkey, Tvalue> MergeDictionaries<Tkey, Tvalue>(IEnumerable<Dictionary<Tkey, Tvalue>> dicts)
    { 
        IEnumerable<KeyValuePair<Tkey, Tvalue>> dictsIEnum = dicts.SelectMany(dict => dict);
        return dictsIEnum.ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}
