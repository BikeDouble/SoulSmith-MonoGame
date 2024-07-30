using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FormsObject : IDeepCloneable
{
    private List<FormsObject> _children;

    public virtual object DeepClone()
    {
        return new FormsObject(this);
    }

    public FormsObject()
    {
        _children = new List<FormsObject>();
    }

    public FormsObject(FormsObject other)
    {
        _children = new();

        foreach (FormsObject child in other._children)
        {
            _children.Add((FormsObject)child.DeepClone());
        }
    }

    public virtual void Process(double delta)
    {
        foreach (FormsObject child in _children)
        {
            child.Process(delta);
        }
    }

    public virtual void AddChild(FormsObject child)
    {
        if (child == null)
            return;

        if (_children.Contains(child))
        {
            return;
        }

        _children.Add(child);
    }

    public virtual void RemoveChild(FormsObject child)
    {
        if (_children.Contains(child))
        {
            _children.Remove(child);
        }
    }

    public virtual void Draw(Position parentAbsolutePosition, SpriteBatch spriteBatch, DrawableResource activeResource = null)
    {

    }

    public ReadOnlyCollection<FormsObject> Children { get { return _children.AsReadOnly(); } }
    public int ChildCount { get { return _children.Count; } }
}
