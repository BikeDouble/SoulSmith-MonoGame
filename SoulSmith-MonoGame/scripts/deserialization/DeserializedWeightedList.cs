
using System;
using System.Collections;
using System.Collections.Generic;

namespace SoulSmithDeserialization;
public class DeserializedWeightedList 
{
   public DeserializedWeightedListItem[] Items { get; set; }
}

public class DeserializedWeightedListItem
{
    public string Name { get; set; }
    public int Weight { get; set; }
}

public class DeserializedMasterEnemySpawnList
{
    public DeserializedEnemySpawnList[] Lists { get; set; }
}

public class DeserializedEnemySpawnList
{
    public string Name { get; set; }
    public int ActivationRound { get; set; }
}