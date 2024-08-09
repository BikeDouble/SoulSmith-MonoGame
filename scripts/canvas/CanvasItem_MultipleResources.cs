

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class CanvasItem_MultipleResources : CanvasItem
{
    private ReadOnlyCollection<DrawableResource> _drawableResources = null;
    private int _activeResourceIndex = -1;

    public CanvasItem_MultipleResources() : base() { }

    public CanvasItem_MultipleResources(
        DrawableResource sprite1, 
        DrawableResource sprite2, 
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = null, 
        Position position = null) : base(position, null, boundingZones)  
    {
        List<DrawableResource> drawableResources = new List<DrawableResource> { sprite1, sprite2 };
        _drawableResources = drawableResources.AsReadOnly();
        _activeResourceIndex = 0;
    }

    public CanvasItem_MultipleResources(
        IEnumerable<DrawableResource> sprites,
        Dictionary<BoundingZoneType, CanvasItem> boundingZones = null,
        Position position = null) : base(position, null, boundingZones)
    {
        if ((sprites != null) && (sprites.Count() > 0))
        {
            _drawableResources = sprites.ToList().AsReadOnly();
            _activeResourceIndex = 0;
        }
    }

    public void SetActiveResourceIndex(int index)
    {
        _activeResourceIndex = SetActiveResourceIndexInternal(index);
    }

    private int SetActiveResourceIndexInternal(int index)
    {
        if (index < 0)
            return -1;

        if (_drawableResources == null)
            return -1;

        if (index >= _drawableResources.Count)
            return -1;

        return index;
    }

    public override void Draw(Position absolutePosition, SpriteBatch spriteBatch, DrawableResource overridenResource = null)
    {
        if (overridenResource == null)
        {
            if ((_activeResourceIndex >= 0) && (_drawableResources != null))
                overridenResource = _drawableResources[_activeResourceIndex];
        }

        base.Draw(absolutePosition, spriteBatch, overridenResource);
    }

    protected DrawableResource GetResource(int index)
    {
        if (_drawableResources == null)
            return null;

        if (index < 0)
            return null;

        if (index >= _drawableResources.Count())
            return null;

        return _drawableResources[index];
    }

    protected ReadOnlyCollection<DrawableResource> Resources { get { return _drawableResources; } }
    protected override DrawableResource Resource { get { return GetResource(_activeResourceIndex); } }
}

