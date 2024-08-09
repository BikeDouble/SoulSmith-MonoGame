

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

public class DrawableResource_Text : DrawableResource
{
    private string _text = null;
    private SpriteFont _font;
    private Color _color = Color.Black;

    public DrawableResource_Text() : base() { }

    public DrawableResource_Text(SpriteFont font, string text = null)
    {
        _font = font;
        _text = text;
    }

    public DrawableResource_Text(DrawableResource_Text other) : base(other)
    {
        _font = other._font;
        _text = other._text;
    }

    public override void Draw(Position position, SpriteBatch spriteBatch) 
    {
        if (_font == null)
            return;

        if (_text == null)
            return;

        if (_text.Length == 0)
            return;

        DrawInternal(_text, _font, position, spriteBatch, _color);
    }

    public static void DrawInternal(string text, SpriteFont font, Position position, SpriteBatch spriteBatch, Color color, bool centered = true)
    {
        Vector2 coords = position.Coordinates;

        if (centered)
        {
            coords -= font.MeasureString(text) / 2;
        }

        spriteBatch.DrawString(font, text, coords, color, position.Rotation, Vector2.Zero, position.Scale, SpriteEffects.None, 0);
    }

    public override object DeepClone()
    {
        return new DrawableResource_Text(this);
    }

    public override void UpdateText(string text)
    {
        _text = text;
    }

    public override void UpdateColor(Color color)
    {
        _color = color;
    }
}

