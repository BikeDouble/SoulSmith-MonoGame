
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

public class DrawableResource_Texture2D : DrawableResource
{
    private TrackedResource<Texture2D> _texture;

    public DrawableResource_Texture2D(DrawableResource_Texture2D other) : base(other)
    {
        _texture = new TrackedResource<Texture2D>(other._texture);
    }

    public DrawableResource_Texture2D(TrackedResource<Texture2D> texture)
    {
        _texture = texture;
    }

    public override object DeepClone()
    {
        return new DrawableResource_Texture2D(this);
    }

    public override void Draw(Position position, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
                _texture,
                position.Coordinates,
                null,
                Color.White,
                position.Rotation,
                new Vector2(0, 0),
                position.ScaleAsVector2(),
                SpriteEffects.None,
                0f);
    }
}

