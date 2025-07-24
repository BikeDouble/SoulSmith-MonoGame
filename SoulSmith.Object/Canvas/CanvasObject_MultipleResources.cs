

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SoulSmith.Drawing;
using SoulSmith.Core;
using SoulSmith.Asset;

namespace SoulSmith.Object.Canvas;
public class CanvasObject_MultipleResources : CanvasObject
{
    private ReadOnlyCollection<IDrawableResource> _drawableResources = null;
    private int _activeResourceIndex = -1;

    public CanvasObject_MultipleResources(
        IDrawableResource sprite1,
        IDrawableResource sprite2,
        Position position = null) : base(position, null)
    {
        List<IDrawableResource> drawableResources = new List<IDrawableResource> { sprite1, sprite2 };
        _drawableResources = drawableResources.AsReadOnly();
        _activeResourceIndex = 0;
    }

    public CanvasObject_MultipleResources(
        IEnumerable<IDrawableResource> sprites,
        Position position = null) : base(position, null)
    {
        if (sprites != null && sprites.Count() > 0)
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

    public override void Dispose()
    {
        foreach (var sprite in _drawableResources) { sprite.Dispose(); }

        base.Dispose();
    }

    protected IDrawableResource GetResource(int index)
    {
        if (_drawableResources == null)
            return null;

        if (index < 0)
            return null;

        if (index >= _drawableResources.Count())
            return null;

        return _drawableResources[index];
    }

    protected ReadOnlyCollection<IDrawableResource> Resources { get { return _drawableResources; } }
    protected override IDrawableResource Resource { get { return GetResource(_activeResourceIndex); } }
}

