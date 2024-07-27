
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

public partial class AssetManager<T> where T : class
{
    private Dictionary<string, List<string>> _assetGroupDict;
    private Dictionary<string, Asset<T>> _assetDict;
    private Func<string, T> _loader;

    public AssetManager(string jsonFilePath, Func<string, T> loader)
    {
        _loader = loader;

        string jsonString = File.ReadAllText(jsonFilePath);
        DeserializedAssetList deserializedAssetList = JsonSerializer.Deserialize<DeserializedAssetList>(jsonString);

        //IList<(string Name, IList<(string Name, string AssetPath)> Assets)> assetGroups = deserializedAssetList.AssetGroups;
        IList<DeserializedAssetGroup> assetGroups = deserializedAssetList.AssetGroups;

        _assetGroupDict = new Dictionary<string, List<string>>();
        _assetDict = new Dictionary<string, Asset<T>>();

        if (assetGroups == null)
        {
            Trace.TraceError("Json file reading error: " + jsonFilePath);
        }
        else
        {
            foreach (DeserializedAssetGroup group in assetGroups)
            {
                List<string> assetGroup = new List<string>();

                if (_assetGroupDict.ContainsKey(group.Name.ToLower()))
                {
                    Trace.TraceError("Duplicate asset group in asset manager creation: " + group.Name);
                }
                else
                {
                    foreach (DeserializedAsset asset in group.Assets)
                    {
                        assetGroup.Add(asset.Name.ToLower());
                    }
                    _assetGroupDict.Add(group.Name.ToLower(), assetGroup);
                }

                foreach (DeserializedAsset asset in group.Assets)
                {
                    if (_assetDict.ContainsKey(asset.Name.ToLower()))
                    {
                        Trace.TraceError("Duplicate asset name: " + asset.Name);
                    }
                    else
                    {
                        Asset<T> newAsset = new Asset<T>(asset.AssetPath);
                        newAsset.LoadAssetEventHandler += LoadAsset;
                        _assetDict.Add(asset.Name.ToLower(), newAsset);
                    }
                }
            }

        }
    }

    /// <summary>
    /// Listens to all assets managed by asset manager
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadAsset(object sender, LoadAssetEventArgs<T> e)
    {
        T asset = _loader(e.AssetPath);
        e.Asset = asset;
    }

    // Loads all assets in the named asset list, if it exists
    public void PreloadAssetGroup(string name)
    {
        if (_assetGroupDict.ContainsKey(name.ToLower()))
        {
            List<string> assetGroup = _assetGroupDict.GetValueOrDefault(name.ToLower());
            foreach (string assetName in assetGroup)
            {
                Asset<T> asset = null;
                _assetDict.TryGetValue(assetName, out asset);
                if (asset != null)
                {
                    asset.Preload();
                }
            }
        }
    }

    public void PreloadAll()
    {
        foreach (KeyValuePair<string, Asset<T>> pair in _assetDict)
        {
            pair.Value.Preload();
        }
    }

    // Unloads all assets in the named asset list, if it exists
    public void UnpreloadAssetGroup(string name)
    {
        if (_assetGroupDict.ContainsKey(name))
        {
            List<string> assetGroup = _assetGroupDict.GetValueOrDefault(name);
            foreach (string assetName in assetGroup)
            {
                Asset<T> asset = null;
                _assetDict.TryGetValue(assetName, out asset);
                if (asset != null)
                {
                    asset.Unpreload();
                }
            }
        }
    }

    public void UnpreloadAll()
    {
        foreach (KeyValuePair<string, Asset<T>> pair in _assetDict)
        {
            pair.Value.Unpreload();
        }
    }

    // Get a specific asset
    public TrackedResource<T> GetAsset(string assetName)
    {
        Asset<T> asset = null;
        _assetDict.TryGetValue(assetName.ToLower(), out asset);

        if (asset != null)
        {
            TrackedResource<T> wrappedAsset = asset.GetAsset();

            if (wrappedAsset == null)
            {
                Trace.TraceError("Asset found with name: " + assetName + ", however, asset was unable to load");
            }

            return wrappedAsset;
        }

        Trace.TraceError("No asset found with name: " + assetName);
        return null;
    }

    public void UnloadUnusedAssets()
    {
        foreach (KeyValuePair<string, Asset<T>> pair in _assetDict)
        {
            pair.Value.UnloadIfUnused();
        }
    }

}

