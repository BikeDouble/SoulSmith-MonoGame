using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MonoGame.Extended.Graphics;

public class CanvasItem : SoulSmithObject, IReadOnlyCanvasItem, ITransformable
{
    private bool _visible = true;
    private string _resourceType = "none";
    private CanvasPosition _position = null;  
    private Vector4 _tint = Vector4.Zero;
    private DrawableResource _drawableResource = null;
    private Dictionary<BoundingZoneType, CanvasItem> _boundingZones = null;

    public CanvasItem(
        CanvasPosition position = null,
        DrawableResource sprite = null,
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = null, 
        IEnumerable<SoulSmithObject> children = null) : base(children)
    {
        _position = new CanvasPosition(position);
        _boundingZones = boundingZones;
        AddChildrenInBoundingZones();

        if (sprite != null)
        {
            _drawableResource = sprite;
        }
    }

    public CanvasItem(int x, int y)
    {
        _position = new CanvasPosition(x, y);
    }

    public CanvasItem(SpriteFont font, string text = null, CanvasPosition position = null)
    {
        _position = new CanvasPosition(position);

        if (font != null)
        {
            _drawableResource = new DrawableResource_Text(font, text);
        }
    }

    public CanvasItem(CanvasItem other, CanvasItem shelledItem = null) : base(other)
    {
        _position = new CanvasPosition(other._position);
        _visible = other._visible;

        _drawableResource = (DrawableResource)other._drawableResource?.DeepClone();
        _boundingZones = CloneBoundingZones(other.BoundingZones, Children, other.Children);

        if (shelledItem != null)
        {
            AddChild(shelledItem);

            if (shelledItem.BoundingZones != null)
            {
                foreach (KeyValuePair<BoundingZoneType, CanvasItem> item in shelledItem.BoundingZones)
                {
                    _boundingZones.TryAdd(item.Key, shelledItem);
                }
            }
        }

        foreach (SoulSmithObject child in Children)
        {
            if (child is CanvasItem)
            {
                RegisterChildEvents((CanvasItem)child);
            }
        }
    }

    private static Dictionary<BoundingZoneType, CanvasItem> CloneBoundingZones(
        ReadOnlyDictionary<BoundingZoneType, CanvasItem> otherZones, 
        ReadOnlyCollection<SoulSmithObject> children,
        ReadOnlyCollection<SoulSmithObject> otherChildren)
    {
        if (otherZones == null) return new();

        Dictionary<BoundingZoneType, CanvasItem> boundingZones = new();

        foreach (KeyValuePair<BoundingZoneType, CanvasItem> pair in otherZones) 
        {
            int zoneIndex = otherChildren.IndexOf(pair.Value);

            CanvasItem zone = children[zoneIndex] as CanvasItem;

            if (zone != null)
                boundingZones.TryAdd(pair.Key, zone);
        }

        if (boundingZones.Count < 1) return null;

        return boundingZones;
    }

    public event EventHandler<GetGlobalPositionEventArgs> GetGlobalPositionEventHandler;

    public CanvasPosition GetGlobalPosition()
    {
        GetGlobalPositionEventArgs e = new GetGlobalPositionEventArgs();

        e.Position = new CanvasPosition();

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

    private void AddChildrenInBoundingZones()
    {
        if (_boundingZones == null) return;

        foreach (KeyValuePair<BoundingZoneType, CanvasItem> item in _boundingZones)
        {
            if (!Children.Contains(item.Value)) AddChild(item.Value);
        }
    }

    public virtual Vector2 GetRandomBoundingPointLocal(BoundingZoneType zoneType)
    {
        if (zoneType == BoundingZoneType.None)
            return Vector2.Zero;

        if (_boundingZones == null)
            return Vector2.Zero;

        CanvasItem zone = _boundingZones.GetValueOrDefault(zoneType);

        if (zone == null)
            return Vector2.Zero;

        return zone.GetRandomBoundingPointLocal(zoneType);
    }

    public virtual Vector2 GetRandomBoundingPointGlobal(BoundingZoneType zoneType)
    {
        if (zoneType == BoundingZoneType.None)
            return Vector2.Zero;

        if (_boundingZones == null)
            return GetGlobalPosition().Coordinates + Vector2.Zero;

        CanvasItem zone = _boundingZones.GetValueOrDefault(zoneType);

        if (zone == null)
            return Vector2.Zero;

        return zone.GetRandomBoundingPointGlobal(zoneType);
    }

    public bool ContainsPointRelative(Vector2 point)
    {
        if (Resource is null)
            return false;

        return Resource.ContainsPoint(point, Position);
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

    public void Set(CanvasPosition position)
    {
        if (position == null)
            return;

        _position.Set(position);
    }

    public void Set(Vector2 coordinates)
    {
        _position.Set(coordinates);
    }

    public void Transform(CanvasPosition transformation)
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

    public void ScaleMultiplicative(Vector2 scale)
    {
        if (scale == Vector2.One) return;

        _position.ScaleMultiplicative(scale);
    }
    public void ScaleAdditive(Vector2 scale)
    {
        if (scale == Vector2.Zero) return;

        _position.ScaleAdditive(scale);
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

    public override void CollectDrawPackets(CanvasPosition absolutePosition, Vector4 tint, IAddOnly<DrawPacket> renderQueue, Rectangle? scissorRect = null)
    {
        tint += _tint;

        CanvasPosition newPosition;

        newPosition = new CanvasPosition(absolutePosition);
        newPosition.Transform(_position);

        DrawableResource resourceToDraw = Resource;

        if ((resourceToDraw != null) && (_visible))
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

    public override void CollectInputPackets(CanvasPosition parentAbsolutePosition, IAddOnly<InputPacket> inputQueue, CanvasPosition absolutePosition = null)
    {
        CanvasPosition newPosition = absolutePosition; 

        if (newPosition == null)
        {
            newPosition = new CanvasPosition(parentAbsolutePosition);
            newPosition.Transform(_position);
        }

        base.CollectInputPackets(newPosition, inputQueue);
    }

    public override InputPacket CreateInputPacket(Func<InputPacketFuncInput, InputPacketFuncOutput> func, IReadOnlyCanvasPosition absPos = null, bool requestHover = false, int priority = 0)
    {
        CanvasItem clickBox = _boundingZones?.GetValueOrDefault(BoundingZoneType.ButtonClickBox);
        if (clickBox == null) { clickBox = this; } //TODO rework this

        InputPacket packet = new(
            clickBox,
            func,
            priority,
            this,
            absPos,
            requestHover);

        return packet;
    }

    public override void AddChild(SoulSmithObject child)
    {
        if (Children.Contains(child))
            return;

        if (child is CanvasItem) 
        {
            RegisterChildEvents((CanvasItem)child);
        }

        base.AddChild(child);
    }

    public override void RemoveChild(SoulSmithObject child)
    {
        if (!Children.Contains(child))
            return;

        if (child is CanvasItem)
        {
            DeRegisterChildEvents((CanvasItem)child);
            RemoveBoundingZones((CanvasItem)child);
        }

        base.RemoveChild(child);
    }

    public virtual void UpdateText(string text)
    {
        if (_drawableResource != null)
        {
            _drawableResource.UpdateText(text);
        }
    }

    private void RegisterChildEvents(CanvasItem child)
    {
        child.GetGlobalPositionEventHandler += GetGlobalPositionInternal;
        child.GetGlobalVisibilityEventHandler += IsVisibleInternal;
    }

    private void DeRegisterChildEvents(CanvasItem child)
    {
        child.GetGlobalPositionEventHandler -= GetGlobalPositionInternal;
        child.GetGlobalVisibilityEventHandler -= IsVisibleInternal;
    }

    /// <summary>
    /// Removes child from objects bounding zone dictionary.
    /// </summary>
    /// <param name="child"></param>
    private void RemoveBoundingZones(CanvasItem child)
    {
        if ((_boundingZones == null) || (_boundingZones.Count == 0))
            return;

        if (_boundingZones.ContainsValue(child))
        {
            List<BoundingZoneType> badKeys = new();
            foreach (KeyValuePair<BoundingZoneType, CanvasItem> pair in _boundingZones)
            {
                if (pair.Value == child)
                    badKeys.Add(pair.Key);
            }

            foreach (BoundingZoneType badKey in badKeys)
            {
                _boundingZones.Remove(badKey);
            }
        }
    }

    public override object DeepClone()
    {
        return new CanvasItem(this);
    }

    public bool Visible { get { return _visible; } }
    public IReadOnlyCanvasPosition Position { get { return _position; } }
    protected virtual DrawableResource Resource { get { return _drawableResource; } set { _drawableResource = value; } }
    public ReadOnlyDictionary<BoundingZoneType, CanvasItem> BoundingZones { get { return _boundingZones == null ? null : new ReadOnlyDictionary<BoundingZoneType, CanvasItem>(_boundingZones); } }
}

public enum BoundingZoneType
{
    None = 0,
    EffectSender,
    EffectReceiver,
    ButtonClickBox
}

public class GetGlobalPositionEventArgs : EventArgs
{
    public CanvasPosition Position { get; set; }
}

public class GetGlobalVisibilityEventArgs : EventArgs
{
    public bool Visible { get; set; }
}

