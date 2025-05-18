
// Extension of KaimiraGames.WeightedList with some added functions

using KaimiraGames;
using System;
using System.Collections.Generic;
using System.Linq;
using SoulSmith.Core;

public class SoulSmithWeightedList<T> : WeightedList<T>, IDeepCloneable
{
    public SoulSmithWeightedList(ICollection<WeightedListItem<T>> listItems, Random rand) : base(listItems, rand) { }

    public List<T> NextMultiple(int amount)
    {
        if (amount < 0) return null;
        if (amount > Count) amount = Count;

        SoulSmithWeightedList<T> tempList = (SoulSmithWeightedList<T>)DeepClone();
        List<T> returnList = new List<T>();

        for (int i = 0; i < amount; i++)
        {
            T item = tempList.Next();
            returnList.Add(item);
            tempList.Remove(item);
        }

        return returnList;
    }

    public object DeepClone()
    {
        List<WeightedListItem<T>> list = new List<WeightedListItem<T>>();
        foreach (T item in this) 
        {
            WeightedListItem<T> weightedItem = new WeightedListItem<T> (item, GetWeightOf(item));
            list.Add(weightedItem);
        }
        
        return new SoulSmithWeightedList<T>(list, Rand.Random);
    }
}

