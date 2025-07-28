using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Object.Canvas;
using SoulSmith.Battle;
using SoulSmith.Drawing;
using Microsoft.Xna.Framework;

namespace SoulSmith.Game;

public class UnitListUIEntry : CanvasObject
{
    public const int WIDTH = 300;
    public const int HEIGHT = 100;
    public const int PADDING = 10;

    private IReadOnlyUnit _unit;
    private CanvasObject _unitDisplaySprite;

    public UnitListUIEntry(
        IReadOnlyUnit unit)
    {
        _unit = unit;

        Initialize();
    }

    private void Initialize()
    {
        InitializeDisplaySprite();
    }

    private void InitializeDisplaySprite()
    {
        IDrawableResource spriteResource = DrawHelpers.GetDrawableResourceInstance(_unit.SpriteKey);
        int spriteWidth = HEIGHT - (2 * PADDING);
        _unitDisplaySprite = new CanvasObject(new Core.Position(PADDING + spriteWidth / 2, PADDING + spriteWidth / 2),
            spriteResource);
        _unitDisplaySprite.ScaleToSetSize(new Vector2(spriteWidth, spriteWidth));
    }

    public override void Dispose()
    {
        base.Dispose();

        _unit = null;
    }

    public IReadOnlyUnit Unit { get { return _unit; } }
}

