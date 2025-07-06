using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Shapes;
using SoulSmith.Asset;
using SoulSmith.Input;

namespace SoulSmith.Object.Canvas;
public class CanvasObject : SoulSmithObject, IReadOnlyCanvasObject, ITransformable, ICanvasObject
{
    private bool _visible = true;
    private Position _position = null;
    private Vector4 _tint = Vector4.Zero;
    private IAssetWrapper<IDrawableResource> _wrappedResource = null;

    public CanvasObject(
        Position position = null,
        IAssetWrapper<IDrawableResource> sprite = null,
        IEnumerable<SoulSmithObject> children = null) : base(children)
    {
        _position = new Position(position);

        if (sprite != null)
        {
            _wrappedResource = sprite;
        }
    }

    public CanvasObject(int x, int y)
    {
        _position = new Position(x, y);
    }

    public CanvasObject(SpriteFont font, string text = null, Position position = null)
    {
        _position = new Position(position);

        if (font != null)
        {
            _wrappedResource = null;// new DrawableResource_Text(font, text); TODO fix fonts
        }
    }

    public event EventHandler<GetGlobalPositionEventArgs> GetGlobalPositionEventHandler;

    public Position GetGlobalPosition()
    {
        GetGlobalPositionEventArgs e = new GetGlobalPositionEventArgs();

        e.Position = new Position();

        GetGlobalPositionInternal(this, e);

        return e.Position;
    }

    private void GetGlobalPositionInternal(object sender, GetGlobalPositionEventArgs e)
    {
        if (e == null)
            return;

        e.Position += _position;

        GetGlobalPositionEventHandler?.Invoke(this, e);
    }

    public bool ContainsPointRelative(Vector2 point)
    {
        if (Resource is null)
            return false;

        IZone zone = Resource as IZone;

        if (zone == null) return false;

        return zone.ContainsGlobal(point, Position);
    }

    public event EventHandler<GetGlobalVisibilityEventArgs> GetGlobalVisibilityEventHandler;

    public bool IsVisible()
    {
        if (!_visible)
            return false;

        GetGlobalVisibilityEventArgs e = new GetGlobalVisibilityEventArgs();

        e.Visible = true;

        IsVisibleInternal(this, e);

        return e.Visible;
    }

    private void IsVisibleInternal(object sender, GetGlobalVisibilityEventArgs e)
    {
        if (e == null)
            return;

        if (!_visible)
        {
            e.Visible = false;
            return;
        }

        GetGlobalVisibilityEventHandler?.Invoke(this, e);
    }

    public void Show()
    {
        _visible = true;
    }

    public void Hide()
    {
        _visible = false;
    }

    public override void Process(double delta)
    {
        base.Process(delta);    

        Resource?.Process(delta);
    }

    public void Set(Position position)
    {
        if (position == null)
            return;

        _position.Set(position);
    }

    public void Set(Vector2 coordinates)
    {
        _position.Set(coordinates);
    }

    public void Transform(IReadOnlyPosition transformation)
    {
        if (transformation == null)
            return;

        _position.Transform(transformation);
    }

    public void Translate(Vector2 translation)
    {
        if (translation == Vector2.Zero) return;

        _position.Translate(translation);
    }

    public void Scale(Vector2 scale)
    {
        if (scale == Vector2.One) return;

        _position.Scale(scale);
    }

    public void SetScale(Vector2 scale)
    {
        _position.SetScale(scale);
    }

    public void Rotate(float rotation)
    {
        if (rotation == 0) return;

        _position.Rotate(rotation);
    }

    public void Rotate(float rotation, Vector2? origin = null)
    {
        if (rotation == 0) return;

        _position.Rotate(rotation, origin);
    }

    public void ChangeTintAdditive(float r, float g, float b, float a)
    {
        Vector4 tintChange = new(r, g, b, a);

        ChangeTintAdditive(tintChange);
    }

    public void ChangeTintAdditive(Vector4 change)
    {
        _tint += change;
    }

    public override void CollectDrawPackets(IReadOnlyPosition absolutePosition, Vector4 tint, IAddOnly<DrawPacket> renderQueue, Rectangle? scissorRect = null)
    {
        tint += _tint;

        Position newPosition;

        newPosition = new Position(absolutePosition);
        newPosition.Transform(_position);

        IDrawableResource resourceToDraw = Resource;

        if (resourceToDraw != null && _visible)
        {
            renderQueue.Add(new DrawPacket(newPosition, tint, Resource, scissorRect));
        }

        if (_visible)
        {
            foreach (SoulSmithObject child in Children)
            {
                child.CollectDrawPackets(newPosition, tint, renderQueue, scissorRect);
            }
        }

        base.CollectDrawPackets(absolutePosition, tint, renderQueue, scissorRect);
    }

    public override void CollectInputPackets(IReadOnlyPosition parentAbsolutePosition, IAddOnly<InputPacket> inputQueue, Position absolutePosition = null)
    {
        Position newPosition = absolutePosition; //TODO investigate, rework

        if (newPosition == null)
        {
            newPosition = new Position(parentAbsolutePosition);
            newPosition.Transform(_position);
        }

        base.CollectInputPackets(newPosition, inputQueue);
    }

    public override void AddChild(SoulSmithObject child)
    {
        if (Children.Contains(child))
            return;

        if (child is CanvasObject)
        {
            RegisterChildEvents((CanvasObject)child);
        }

        base.AddChild(child);
    }

    public override void RemoveChild(SoulSmithObject child)
    {
        if (!Children.Contains(child))
            return;

        if (child is CanvasObject)
        {
            DeRegisterChildEvents((CanvasObject)child);
        }

        base.RemoveChild(child);
    }

    public virtual void UpdateText(string text)
    {
        if (_wrappedResource != null)
        {
            //_drawableResource.UpdateText(text); TODO fix fonts
        }
    }

    public void UpdateResourceState(string newState)
    {
        Resource.UpdateState(newState);
    }

    private void RegisterChildEvents(CanvasObject child)
    {
        child.GetGlobalPositionEventHandler += GetGlobalPositionInternal;
        child.GetGlobalVisibilityEventHandler += IsVisibleInternal;
    }

    private void DeRegisterChildEvents(CanvasObject child)
    {
        child.GetGlobalPositionEventHandler -= GetGlobalPositionInternal;
        child.GetGlobalVisibilityEventHandler -= IsVisibleInternal;
    }

    public override void Dispose()
    {
        _wrappedResource.Dispose();

        base.Dispose();
    }

    public bool Visible { get { return _visible; } }
    public IReadOnlyPosition Position { get { return _position; } }
    protected virtual IDrawableResource Resource { get { return _wrappedResource?.Value; } }
}

public class GetGlobalPositionEventArgs : EventArgs
{
    public Position Position { get; set; }
}

public class GetGlobalVisibilityEventArgs : EventArgs
{
    public bool Visible { get; set; }
}

