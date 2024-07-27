
using System;
using System.Collections;
using System.Collections.Generic;

public class DeserializedAssetList 
{
   public DeserializedAssetGroup[] AssetGroups { get; set; }
}

public class DeserializedAssetGroup
{
    public string Name { get; set; }
    public DeserializedAsset[] Assets { get; set; }
}

public class DeserializedAsset
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string AssetPath { get; set; }
}