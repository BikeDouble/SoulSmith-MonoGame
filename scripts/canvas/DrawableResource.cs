

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class DrawableResource : IDeepCloneable
{
    public DrawableResource() { }

    public DrawableResource(DrawableResource other) { }

    public virtual void Draw(Position position, SpriteBatch spriteBatch) { }

    public virtual void UpdatePosition(Position position) { } 

    public virtual void UpdateText(string text) { }

    public virtual void UpdateColor(Color color) { }

    public virtual object DeepClone()
    {
        return new DrawableResource(this);
    }

    public virtual bool ContainsPoint(Vector2 point, Position position)
    {
        return false;
    }
}

