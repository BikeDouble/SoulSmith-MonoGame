using Microsoft.Xna.Framework;
using SoulSmith.Core;
using SoulSmith.Drawing;
using SoulSmith.Drawing.Zoned;
using SoulSmith.Input;
using SoulSmith.Shapes;

namespace SoulSmith.Object.Canvas;
public class CanvasObject : SoulSmithObject, IReadOnlyCanvasObject
{
    private bool _visible = true;
    private Position _position = null;
    private Color _color = Color.White;
    private IDrawableResource _drawableResource = null;

    public CanvasObject(
        string drawableResourceKey,
        Position position = null,
        IEnumerable<SoulSmithObject> children = null) : 
        this(
            position,
            DrawHelpers.GetDrawableResourceInstance(drawableResourceKey),
            children)
    {}

    public CanvasObject(
        Position position = null,
        IDrawableResource drawableResource = null,
        IEnumerable<SoulSmithObject> children = null) : base(children)
    {
        _position = new Position(position);

        if (drawableResource != null)
        {
            _drawableResource = drawableResource;
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

        Position newPosition = new Position();
        newPosition.TransformInContext(e.Position, this.Position);
        newPosition.Transform(this.Position);

        e.Position = newPosition;

        GetGlobalPositionEventHandler?.Invoke(this, e);
    }

    public bool ContainsPointRelative(Vector2 point)
    {
        if (Resource is null)
            return false;

        IMultiZone zone = Resource as IMultiZone;

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
    public void ScaleToSetSize(Vector2 desiredSize, bool preserveRatio = true, Vector2? trueResourceSize = null)
    {
        if (Resource == null && !trueResourceSize.HasValue) return;

        Vector2 resourceSize = trueResourceSize ?? Resource.Size;

        if (resourceSize == Vector2.Zero) return;

        if (desiredSize == Vector2.Zero)
        {
            this.SetScale(Vector2.Zero);
            return;
        }

        Vector2 desiredScale = desiredSize / resourceSize;

        if (preserveRatio)
        {
            float minScale = Math.Min(desiredScale.X, desiredScale.Y);
            desiredScale = new Vector2(minScale, minScale);
        }

        this.SetScale(desiredScale);
    }

    public void Set(IReadOnlyPosition position)
    {
        if (position == null)
            return;

        this.SetCoordinates(position.Coordinates);
        this.SetScale(position.ScaleVector);
        this.SetRotation(position.Rotation);
        this.SetZ(position.Z);
    }

    public void SetCoordinates(Vector2 coordinates)
    {
        Vector2 difference = coordinates - _position.Coordinates;
        this.Translate(difference);
    }

    public void SetZ(int z)
    {
        if (z == _position.Z) return;
        int difference = z - _position.Z;
        this.ZTranslate(difference);
    }

    public void SetRotation(float rotation)
    {
        if (rotation == _position.Rotation) return;
        int difference = (int)(rotation - _position.Rotation);
        this.Rotate(difference);
    }

    public void SetScale(Vector2 scale)
    {
        Vector2 change = scale / _position.ScaleVector;

        this.Scale(change);
    }

    public void Transform(IReadOnlyPosition transformation)
    {
        if (transformation == null)
            return;

        this.Translate(transformation.Coordinates);
        this.Scale(transformation.ScaleVector);
        this.Rotate(transformation.Rotation);
        this.ZTranslate(transformation.Z);
    }

    public void Translate(Vector2 translation)
    {
        if (translation == Vector2.Zero) return;

        _position.Translate(translation);
    }

    public void ZTranslate(int zTranslation)
    {
        if (zTranslation == 0) return;
        _position.ZTranslate(zTranslation);
    }

    public void Scale(float scale)
    {
        if (scale == 1) return;

        this.Scale(new Vector2(scale));
    }

    public void Scale(Vector2 scale)
    {
        if (scale == Vector2.One) return;

        _position.Scale(scale);
    }

    public void Rotate(float rotation)
    {
        if (rotation == 0) return;

        _position.Rotate(rotation);
    }

    public void Rotate(float rotation, Vector2 origin)
    {
        if (rotation == 0) return;

        _position.Rotate(rotation, origin);

        throw new NotImplementedException();
    }

    public void ChangeColorAdditive(int r, int g, int b, int a)
    {
        byte newR = (byte)Math.Clamp((int)_color.R + r, 0, 255);
        byte newG = (byte)Math.Clamp((int)_color.G + g, 0, 255);
        byte newB = (byte)Math.Clamp((int)_color.B + b, 0, 255);
        byte newA = (byte)Math.Clamp((int)_color.A + a, 0, 255);

        _color = new Color(newR, newG, newB, newA);
    }

    public void SetColor(int r, int g, int b, int a)
    {
        SetColor(new Color(r, g, b, a));
    }

    public void SetColor(Color color)
    {
        _color = color;
    }

    public override void CollectDrawPackets(IReadOnlyPosition absolutePosition, Color color, IAddOnly<DrawPacket> renderQueue, Rectangle? scissorRect = null)
    {
        color *= _color;

        Position newPosition;

        newPosition = new Position(absolutePosition);
        newPosition.TransformInContext(Position, absolutePosition);

        IDrawableResource resourceToDraw = Resource;

        if (resourceToDraw != null && _visible)
        {
            renderQueue.Add(new DrawPacket(newPosition, color, Resource, scissorRect, Resource?.SamplerState));
        }

        if (_visible)
        {
            if (!scissorRect.HasValue || ((scissorRect.Value.X != 0) && (scissorRect.Value.Y != 0))) { // Don't keep drawing if scissor rect has zero width: nothing will be visible
                foreach (SoulSmithObject child in Children)
                {
                    child.CollectDrawPackets(newPosition, color, renderQueue, scissorRect);
                }
            }
        }

        base.CollectDrawPackets(absolutePosition, color, renderQueue, scissorRect);
    }

    public override void CollectInputPackets(IReadOnlyPosition parentAbsolutePosition, IAddOnly<InputPacket> inputQueue)
    {
        Position newPosition;

        newPosition = new Position(parentAbsolutePosition);
        newPosition.TransformInContext(_position, parentAbsolutePosition);

        this.CollectInputPacketsInternal(newPosition, inputQueue);

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

    public void UpdateResourceState(string newState, bool force = false)
    {
        Resource?.UpdateState(newState, force);
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
        _drawableResource?.Dispose();

        base.Dispose();
    }

    public virtual void SetOriginPlacement(OriginPlacement originPlacement)
    {
        if (Resource != null)
        {
            Resource.OriginPlacement = originPlacement;
        }
    }

    //
    // IMultiZone implementation.
    //

    public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation = null)
    {
        return ContainsGlobal(point, transformation, string.Empty);
    }

    public bool ContainsGlobal(Vector2 point, IReadOnlyPosition transformation, string zoneKey)
    {
        if (Resource is ZonedDrawableResourceInstance instance)
        {
            return instance.ContainsGlobal(point, transformation, zoneKey);
        }

        return point == transformation.Coordinates;
    }

    public bool ContainsLocal(Vector2 point)
    {
        return ContainsLocal(point, string.Empty);
    }

    public bool ContainsLocal(Vector2 point, string zoneKey)
    {
        if (Resource is ZonedDrawableResourceInstance instance)
        {
            return instance.ContainsLocal(point, zoneKey);
        }

        return point == Vector2.Zero;
    }

    public Vector2 GetRandomLocalPoint()
    {
        return GetRandomLocalPoint(string.Empty);
    }

    public Vector2 GetRandomLocalPoint(string zoneKey)
    {
        if (Resource is ZonedDrawableResourceInstance instance)
        {
            return instance.GetRandomLocalPoint(zoneKey);
        }

        return Vector2.Zero;
    }

    public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position)
    {
        return GetRandomGlobalPoint(position, string.Empty);
    }

    public Vector2 GetRandomGlobalPoint(IReadOnlyPosition position, string zoneKey)
    {
        if (Resource is ZonedDrawableResourceInstance instance)
        {
            return instance.GetRandomGlobalPoint(position, zoneKey);
        }

        return position.Coordinates;
    }

    public float GetAreaLocal()
    {
        return GetAreaLocal(string.Empty);
    }

    public float GetAreaLocal(string zoneKey)
    {
        if (Resource is ZonedDrawableResourceInstance instance)
        {
            return instance.GetAreaLocal(zoneKey);
        }

        return 0;
    }

    public float GetHeightLocal()
    {
        return GetHeightLocal(string.Empty);
    }

    public float GetHeightLocal(string zoneKey)
    {
        if (Resource is ZonedDrawableResourceInstance instance)
        {
            return instance.GetHeightLocal(zoneKey);
        }

        return 0;
    }

    public float GetWidthLocal()
    {
        return GetWidthLocal(string.Empty);
    }

    public float GetWidthLocal(string zoneKey)
    {
        if (Resource is ZonedDrawableResourceInstance instance)
        {
            return instance.GetWidthLocal(zoneKey);
        }

        return 0;
    }

    public bool Visible { get { return _visible; } }
    public IReadOnlyPosition Position { get { return _position; } }
    protected virtual IDrawableResource Resource { get { return _drawableResource; } }
}

public class GetGlobalPositionEventArgs : EventArgs
{
    public Position Position { get; set; }
}

public class GetGlobalVisibilityEventArgs : EventArgs
{
    public bool Visible { get; set; }
}

