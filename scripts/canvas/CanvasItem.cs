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

public class CanvasItem : FormsObject
{
    private bool _visible = true;
    private string _resourceType = "none";
    public Position Position = null;  
    private DrawableResource _drawableResource = null;
    private Dictionary<BoundingZoneType, CanvasItem> _boundingZones = null;

    public CanvasItem(Position position = null, Dictionary<BoundingZoneType, CanvasItem> boundingZones = null)
    {
        Position = new Position(position);
        _boundingZones = boundingZones;
        AddChildrenInBoundingZones();
    }

    public CanvasItem(int x, int y)
    {
        Position = new Position(x, y);
    }

    public CanvasItem(float[] positionArgs)
    {
        Position = new Position(positionArgs);
    }

    public CanvasItem(DrawableResource sprite, Dictionary<BoundingZoneType, CanvasItem> boundingZones = null, Position position = null)
    {
        Position = new Position(position);

        if (sprite != null)
        {
            _drawableResource = sprite;
            UpdateResourcePosition();
        }

        _boundingZones = boundingZones;
        AddChildrenInBoundingZones();
    }

    public CanvasItem(SpriteFont font, string text = null, Position position = null)
    {
        Position = new Position(position);

        if (font != null)
        {
            _drawableResource = new DrawableResource_Text(font, text);
            UpdateResourcePosition();
        }
    }

    public CanvasItem(CanvasItem other) : base(other)
    {
        Position = new Position(other.Position);
        _visible = other._visible;

        _drawableResource = (DrawableResource)other._drawableResource?.DeepClone();
        _boundingZones = CloneBoundingZones(other.BoundingZones, Children, other.Children);
    }

    private static Dictionary<BoundingZoneType, CanvasItem> CloneBoundingZones(
        ReadOnlyDictionary<BoundingZoneType, CanvasItem> otherZones, 
        ReadOnlyCollection<FormsObject> children,
        ReadOnlyCollection<FormsObject> otherChildren)
    {
        if (otherZones == null) return null;

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

        e.Position += Position;

        GetGlobalPositionEventHandler?.Invoke(this, e);
    }

    private void AddChildrenInBoundingZones()
    {
        if (_boundingZones == null) return;

        foreach (KeyValuePair<BoundingZoneType, CanvasItem> item in _boundingZones)
        {
            AddChild(item.Value);
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
            return Vector2.Zero;

        CanvasItem zone = _boundingZones.GetValueOrDefault(zoneType);

        if (zone == null)
            return Vector2.Zero;

        return zone.GetRandomBoundingPointGlobal(zoneType);
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

    public void Transform(Position transformation)
    {
        Position.Transform(transformation);
        UpdateResourcePosition();
    }

    public void Translate(Vector2 translation)
    {
        Position.Translate(translation);
        UpdateResourcePosition();
    }

    public void Rotate(float rotation)
    {
        Position.Rotate(rotation);
        UpdateResourcePosition();
    }

    public void Rotate(float rotation, Vector2 origin)
    {
        Position.Rotate(rotation, origin);
        UpdateResourcePosition();
    }

    public virtual void UpdateResourcePosition()
    {
        Resource?.UpdatePosition(Position);
    }

    public override void Draw(Position absolutePosition, SpriteBatch spriteBatch, DrawableResource overridenResource = null)
    {
        Position newPosition;

        newPosition = new Position(absolutePosition);
        newPosition.Transform(Position);

        DrawableResource resourceToDraw;

        if (overridenResource != null)
        {
            resourceToDraw = overridenResource;
        }
        else
        {
            resourceToDraw = _drawableResource;
        }

        if ((resourceToDraw != null) && (_visible))
        {
            resourceToDraw.Draw(newPosition, spriteBatch);
        }

        if (_visible)
        {
            foreach (FormsObject child in Children)
            {
                child.Draw(newPosition, spriteBatch);
            }
        }
    }

    public override void AddChild(FormsObject child)
    {
        if (Children.Contains(child))
            return;

        if (child is CanvasItem) 
        {
            RegisterChildEvents((CanvasItem)child);
        }

        base.AddChild(child);
    }

    public override void RemoveChild(FormsObject child)
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
    protected virtual DrawableResource Resource { get { return _drawableResource; } set { _drawableResource = value; } }
    public ReadOnlyDictionary<BoundingZoneType, CanvasItem> BoundingZones { get { return _boundingZones == null ? null : new ReadOnlyDictionary<BoundingZoneType, CanvasItem>(_boundingZones); } }
}

public enum BoundingZoneType
{
    None = 0,
    EffectSender,
    EffectReceiver
}

public class GetGlobalPositionEventArgs : EventArgs
{
    public Position Position { get; set; }
}

public class GetGlobalVisibilityEventArgs : EventArgs
{
    public bool Visible { get; set; }
}

public class Position
{
    public Position()
    {

    }

    public Position(int x = 0, int y = 0, float width = 1f, float height = 1f, float rotation = 0f)
    {
        Width = width;
        Height = height;
        X = x;
        Y = y;
        Rotation = rotation;
    }

    public Position(Position other)
    {
        if (other == null)
            return;

        Scale = other.Scale;
        Coordinates = other.Coordinates;
        Rotation = other.Rotation;
    }

    public Position(float[] positionArgs)
    {
        if (positionArgs.Length >= 5)
        {
            Coordinates = new Vector2(positionArgs[0], positionArgs[1]);
            Scale = new Vector2(positionArgs[2], positionArgs[3]);
            Rotation = positionArgs[4];
        }
    }

    public Position Transform(Position transformation)
    {
        Width *= transformation.Width;
        Height *= transformation.Height;
        X += transformation.X;
        Y += transformation.Y;
        Rotation += transformation.Rotation;

        return this;
    }

    public Position Transform(Vector2 translation)
    {
        return Translate(translation);
    }

    public Position Translate(Vector2 translation)
    {
        Coordinates += translation;

        return this;
    }

    public Position Rotate(float rotation, Vector2 origin)
    {
        Coordinates = RotatePointAroundPoint(Coordinates, origin, rotation);

        return Rotate(rotation);
    }

    public static Vector2 RotatePointAroundPoint(Vector2 point, Vector2 origin, float rotation)
    {
        Vector2 relativePos = point - origin;

        float newX = (float)((relativePos.X * Math.Cos(rotation)) - (relativePos.Y * Math.Sin(rotation)));
        float newY = (float)((relativePos.Y * Math.Cos(rotation)) + (relativePos.X * Math.Sin(rotation)));

        Vector2 newRelativePos = new Vector2(newX, newY);

        return newRelativePos + origin;
    }

    public Position Rotate(float rotation)
    {
        Rotation += rotation;

        return this;
    }

    public Position Set(Position transformation)
    {
        Width = transformation.Width;
        Height = transformation.Height;
        X = transformation.X;
        Y = transformation.Y;
        Rotation = transformation.Rotation;

        return this;
    }

    public Position Set(Vector2 transformation)
    {
        Coordinates = transformation;

        return this;
    }

    public Vector2 ScaleAsVector2()
    {
        return new Vector2((float)Width, (float)Height);
    }

    public const float MAXROTATION = (float)(Math.PI * 2);

    private Vector2 _scale = Vector2.One;
    private Vector2 _coordinates = Vector2.Zero;
    private float _rotation = 0f;

    public Vector2 Scale { get { return _scale; } private set { _scale = value; } }
    public float Width { get { return _scale.X; } private set { _scale.X = value; } }
    public float Height { get { return _scale.Y; } private set { _scale.Y = value; } }
    public Vector2 Coordinates { get { return _coordinates; } private set { _coordinates = value; } }
    public float Rotation { get { return _rotation; } private set { 
            _rotation = value;

            if (_rotation >= MAXROTATION)
            {
                _rotation -= MAXROTATION;
            }

            if (_rotation < 0)
            {
                _rotation += MAXROTATION;
            }
        } }

    public int X { get { return (int)Coordinates.X; } private set { _coordinates.X = value; } }
    public int Y {  get { return (int)Coordinates.Y; } private set { _coordinates.Y = value; } }

    public static Position operator + (Position a, Position b)
       => new Position( a.X + b.X, a.Y + b.Y, a.Width * b.Width, a.Height * b.Height, a.Rotation + b.Rotation);
}