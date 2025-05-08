using Microsoft.Xna.Framework;
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
        child.GetParentEventHandler += ReturnParent;
    }

    public virtual void RemoveChild(SoulSmithObject child)
    {
        if (_children.Contains(child))
        {
            _children.Remove(child);
            child.GetParentEventHandler -= ReturnParent;
        }
    }

    public event EventHandler<GetParentEventArgs> GetParentEventHandler;

    public SoulSmithObject GetParent()
    {
        GetParentEventArgs e = new GetParentEventArgs();

        GetParentEventHandler?.Invoke(this, e);

        return e.Parent;
    }

    private void ReturnParent(object sender, GetParentEventArgs e)
    {
        e.Parent = this;
    }

    public virtual void CollectDrawPackets(CanvasPosition parentAbsolutePosition, Vector4 tint, RenderQueue renderQueue, DrawableResource activeResource = null)
    {

    }

    public ReadOnlyCollection<SoulSmithObject> Children { get { return _children.AsReadOnly(); } }
    public int ChildCount { get { return _children.Count; } }
}

public class GetParentEventArgs : EventArgs
{
    public SoulSmithObject Parent;
}
