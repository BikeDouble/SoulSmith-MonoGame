using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Core;
using SoulSmith.Asset;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoulSmith.Drawing.Animation
{
    [JsonConverter(typeof(AnimationFrameJsonConverter))]
    public class AnimationFrame
    {

        public AnimationFrame(int sourceX, int sourceY, int originX, int originY, int sourceWidth, int sourceHeight, int frameWidth, int frameHeight, string dataName)
        {
            FrameTopLeftRelativeToSourceRect = new Vector2(originX, originY);
            DataName = dataName;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            SourceRect = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
        }

        public readonly Vector2 FrameTopLeftRelativeToSourceRect;
        public Vector2 FrameOrigin { get { return new Vector2(FrameWidth / 2, FrameHeight / 2); } }
        public Vector2 SourceOrigin { get { return new Vector2(SourceRect.Width / 2, SourceRect.Height / 2); } }
        public readonly int FrameWidth;
        public readonly int FrameHeight;
        public readonly string DataName;
        public readonly Rectangle SourceRect;

        public void DrawFrame(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch, IAssetWrapper<Texture2DResource> texture)
        {
            texture.Value.DrawSubsection(position, color, spriteBatch, SourceRect, SourceOrigin);
        }
    }

    public class AnimationFrameJsonConverter : JsonConverter<AnimationFrame>
    {
        public override AnimationFrame Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            int sourceX = 0;
            int sourceY = 0;
            int originX = 0;
            int originY = 0;
            int frameWidth = 0;
            int frameHeight = 0;
            int sourceWidth = 0;
            int sourceHeight = 0;
            string name = string.Empty;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "frameX":
                    case "FrameX":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        originX = reader.GetInt32();
                        reader.Read();
                        break;
                    case "frameY":
                    case "FrameY":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        originY = reader.GetInt32();
                        reader.Read();
                        break;
                    case "x":
                    case "X":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        sourceX = reader.GetInt32();
                        reader.Read();
                        break;
                    case "y":
                    case "Y":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        sourceY = reader.GetInt32();
                        reader.Read();
                        break;
                    case "frameWidth":
                    case "FrameWidth":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        frameWidth = reader.GetInt32();
                        reader.Read();
                        break;
                    case "frameHeight":
                    case "FrameHeight":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        frameHeight = reader.GetInt32();
                        reader.Read();
                        break;
                    case "width":
                    case "Width":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        sourceWidth = reader.GetInt32();
                        reader.Read();
                        break;
                    case "height":
                    case "Height":
                        if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Expected number");
                        sourceHeight = reader.GetInt32();
                        reader.Read();
                        break;
                    case "name":
                        if (reader.TokenType != JsonTokenType.String) throw new JsonException("Expected string");
                        name = reader.GetString();
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new AnimationFrame(sourceX, sourceY, originX, originY, sourceWidth, sourceHeight, frameWidth, frameHeight, name);
        }

        public override void Write(Utf8JsonWriter writer, AnimationFrame value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
