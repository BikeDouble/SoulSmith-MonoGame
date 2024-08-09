using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SoulSmithObject : IDeepCloneable
{
    private List<SoulSmithObject> _children;

    public virtual object DeepClone()
    {
        return new SoulSmithObject(this);
    }

    public SoulSmithObject()
    {
        _children = new List<SoulSmithObject>();
    }

    public SoulSmithObject(IEnumerable<SoulSmithObject> children)
    {
        _children = new List<SoulSmithObject>();
        AddMultipleChildren(children);
    }

    public SoulSmithObject(SoulSmithObject other)
    {
        _children = new();

        foreach (SoulSmithObject child in other._children)
        {
            _children.Add((SoulSmithObject)child.DeepClone());
        }
    }

    public virtual void Process(double delta)
    {
        foreach (SoulSmithObject child in _children)
        {
            child.Process(delta);
        }
    }

    public void AddMultipleChildren(IEnumerable<SoulSmithObject> children)
    {
        if (children != null)
            foreach (SoulSmithObject child in children)
                AddChild(child);
    }

    public virtual void AddChild(SoulSmithObject child)
    {
        if (child == null)
            return;

        if (_children.Contains(child))
        {
            return;
        }

        _children.Add(child);
        child.GetAssetLoaderEventHandler += GetAssetLoaderInternal;
    }

    public virtual void RemoveChild(SoulSmithObject child)
    {
        if (_children.Contains(child))
        {
            child.GetAssetLoaderEventHandler -= GetAssetLoaderInternal;
            _children.Remove(child);
        }
    }

    public event EventHandler<GetAssetLoaderEventArgs> GetAssetLoaderEventHandler;

    public IAssetLoadOnly GetAssetLoader()
    {
        GetAssetLoaderEventArgs e = new GetAssetLoaderEventArgs();

        GetAssetLoaderInternal(this, e);

        return e.AssetLoader;
    }

    public virtual void GetAssetLoaderInternal(object sender, GetAssetLoaderEventArgs e)
    {
        GetAssetLoaderEventHandler?.Invoke(this, e);
    }

    public virtual void Draw(Position parentAbsolutePosition, SpriteBatch spriteBatch, DrawableResource activeResource = null)
    {

    }

    public ReadOnlyCollection<SoulSmithObject> Children { get { return _children.AsReadOnly(); } }
    public int ChildCount { get { return _children.Count; } }
}

public class GetAssetLoaderEventArgs : EventArgs
{
    public IAssetLoadOnly AssetLoader = null;
}