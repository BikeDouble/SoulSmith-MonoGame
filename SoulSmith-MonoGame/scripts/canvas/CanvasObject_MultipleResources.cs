

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Drawing;
using SoulSmith.Core;

public class CanvasObject_MultipleResources : CanvasObject
{
    private ReadOnlyCollection<DrawableResource> _drawableResources = null;
    private int _activeResourceIndex = -1;

    public CanvasObject_MultipleResources() : base() { }

    public CanvasObject_MultipleResources(
        DrawableResource sprite1, 
        DrawableResource sprite2, 
        Dictionary<BoundingZoneType, CanvasObject> boundingZones = null, 
        Position position = null) : base(position, null, boundingZones)  
    {
        List<DrawableResource> drawableResources = new List<DrawableResource> { sprite1, sprite2 };
        _drawableResources = drawableResources.AsReadOnly();
        _activeResourceIndex = 0;
    }

    public CanvasObject_MultipleResources(
        IEnumerable<DrawableResource> sprites,
        Dictionary<BoundingZoneType, CanvasObject> boundingZones = null,
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

