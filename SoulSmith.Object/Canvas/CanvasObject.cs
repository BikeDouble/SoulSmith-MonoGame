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
    private Color _color = Color.White;
    private IAssetWrapper<IDrawableResource> _wrappedResource = null;

    public CanvasObject(
        Position position = null,
        IAssetWrapper<IDrawableResource> drawableResource = null,
        IEnumerable<SoulSmithObject> children = null) : base(children)
    {
        _position = new Position(position);

        if (drawableResource != null)
        {
            _wrappedResource = drawableResource;
        }
    }

    public CanvasObject(int x, int y)
    {
        _position = new Position(x, y);
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

    /// <summary>
    /// Scales object to a desired size in pixels.
    /// </summary>
    /// <param name="desiredSize"></param>
    /// <param name="preserveRatio"></param>
    public void ScaleToSetSize(Vector2 desiredSize, bool preserveRatio = true)
    {
        if (Resource == null) return;

        Vector2 currentSize = Resource.Size;

        if (currentSize == Vector2.Zero || desiredSize == Vector2.Zero) return;

        Vector2 desiredScale = desiredSize / currentSize;

        if (preserveRatio)
        {
            float minScale = Math.Min(desiredScale.X, desiredScale.Y);
            desiredScale = new Vector2(minScale, minScale);
        }

        this.SetScale(desiredScale);
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

    public void ChangeColorAdditive(int r, int g, int b, int a)
    {
        byte newR = (byte)Math.Clamp((int)_color.R + r, 0, 255);
        byte newG = (byte)Math.Clamp((int)_color.G + g, 0, 255);
        byte newB = (byte)Math.Clamp((int)_color.B + b, 0, 255);
        byte newA = (byte)Math.Clamp((int)_color.A + a, 0, 255);

        _color = new Color(newR, newG, newB, newA);
    }

    public override void CollectDrawPackets(IReadOnlyPosition absolutePosition, Color color, IAddOnly<DrawPacket> renderQueue, Rectangle? scissorRect = null)
    {
        color *= _color;

        Position newPosition;

        newPosition = new Position(absolutePosition);
        newPosition.Transform(_position);

        IDrawableResource resourceToDraw = Resource;

        if (resourceToDraw != null && _visible)
        {
            renderQueue.Add(new DrawPacket(newPosition, color, Resource, scissorRect));
        }

        if (_visible)
        {
            foreach (SoulSmithObject child in Children)
            {
                child.CollectDrawPackets(newPosition, color, renderQueue, scissorRect);
            }
        }

        base.CollectDrawPackets(absolutePosition, color, renderQueue, scissorRect);
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

    public void UpdateResourceState(string newState)
    {
        Resource?.UpdateState(newState);
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

