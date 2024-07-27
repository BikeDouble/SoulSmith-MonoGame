using Microsoft.Xna.Framework.Graphics;
using System;

public partial class Asset<T> where T : class
{
	private T _asset = null;
	private string _assetPath;
	private bool _isLoaded;
	private bool _preloaded = false; // If something is preloaded it will not unload when use counter is 0
	private AssetCounter _counter;

	public Asset(string assetPath)
	{
		_assetPath = assetPath;
		_isLoaded = false;
		_counter = new AssetCounter();
	}

	public void Preload()
	{
		_preloaded = true;
		Load();
	}

	public void Unpreload()
	{
		_preloaded = false;
	}

	private void Load()
	{
		if (!_isLoaded) 
		{
			_asset = LoadAssetInternal();
            _isLoaded = true;
        }
	}

	// Unloads iff counter is at 0
	public void UnloadIfUnused()
	{
		if (!_isLoaded)
		{
			return;
		}

		if (_preloaded)
		{
			return;
		}

		if (_counter.Count < 0)
		{
			Unload();
		}
	}

	//Forces an unload, regardless of counter, avoid using
	/*public void ForceUnload()
	{
		Unload();
	}*/

	private void Unload()
	{
        if (_isLoaded)
        {
            _asset = null;
            _isLoaded = false;
        }
    }

	public TrackedResource<T> GetAsset()
	{
		Load();
		return new TrackedResource<T>(_asset, _counter);
	}

    public event EventHandler<LoadAssetEventArgs<T>> LoadAssetEventHandler;

	private T LoadAssetInternal()
	{
		LoadAssetEventArgs<T> e = new LoadAssetEventArgs<T>();

		e.AssetPath = _assetPath;

		LoadAssetEventHandler(this, e);

		return e.Asset;
	}
}

public class LoadAssetEventArgs<T> : EventArgs
{
    public string AssetPath;
    public T Asset;
}

/*public struct AssetIdentifier
{
	public AssetIdentifier(string name, string type)
	{
		Name = name;
		Type = type;
	}

	public readonly string Name;
	public readonly string Type;
}*/
