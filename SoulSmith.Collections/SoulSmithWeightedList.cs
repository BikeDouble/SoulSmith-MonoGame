
// Extension of KaimiraGames.WeightedList with some added functions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SoulSmith.Core;

namespace SoulSmith.Collections;
public class SoulSmithWeightedList<T> : WeightedList<T>, IDeepCloneable, IReadOnlySoulSmithWeightedList<T>
{
    public SoulSmithWeightedList(ICollection<WeightedListItem<T>> listItems, Random rand) : base(listItems, rand) { }

    public List<T> NextMultiple(int amount, bool allowDuplicates = false)
    {
        if (amount < 0) return null;
        if (amount == 1) return new List<T> { this.Next() };
        if ((amount > Count) && (!allowDuplicates)) return new List<T>(this);

        SoulSmithWeightedList<T> tempList = allowDuplicates ? this : (SoulSmithWeightedList<T>)this.DeepClone();

        List<T> returnList = new List<T>();

        for (int i = 0; i < amount; i++)
        {
            T item = tempList.Next();
            returnList.Add(item);
            if (!allowDuplicates) tempList.Remove(item);
        }

        return returnList;
    }

    public object DeepClone()
    {
        List<WeightedListItem<T>> list = new List<WeightedListItem<T>>();
        foreach (T item in this)
        {
            WeightedListItem<T> weightedItem = new WeightedListItem<T>(item, GetWeightOf(item));
            list.Add(weightedItem);
        }

        return new SoulSmithWeightedList<T>(list, Rand.Random);
    }
}

