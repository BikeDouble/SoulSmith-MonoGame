using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDrawableResource
{
    public virtual void Draw(IReadOnlyCanvasPosition position, Vector4 tint, SpriteBatch spriteBatch) { }
}

