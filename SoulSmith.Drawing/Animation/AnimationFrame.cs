using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Core;
using SoulSmith.Asset;
using System.Text.Json;
using System.Text.Json.Serialization;
using SoulSmith.Drawing.Textures;

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
        public readonly int FrameWidth;
        public readonly int FrameHeight;
        public readonly string DataName;
        public readonly Rectangle SourceRect;

        public void DrawFrame(IReadOnlyPosition position, Color color, SpriteBatch spriteBatch, Texture2DInstance textureInstance, OriginPlacement originPlacement = OriginPlacement.Center)
        {
            Vector2 origin = GetSourceOrigin(originPlacement);

            textureInstance.DrawSubsection(position, color, spriteBatch, SourceRect, origin);
        }

        public Vector2 GetFrameOrigin(OriginPlacement originPlacement)
        {
            switch (originPlacement)
            {
                case OriginPlacement.TopLeft:
                    return Vector2.Zero;
                case OriginPlacement.Center:
                    return new Vector2(FrameWidth / 2, FrameHeight / 2);
                case OriginPlacement.BottomMiddle:
                    return new Vector2(FrameWidth / 2, FrameHeight);
                case OriginPlacement.TopMiddle:
                    return new Vector2(FrameWidth / 2, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(originPlacement), originPlacement, "Invalid origin placement specified.");
            }
        }

        public Vector2 GetSourceOrigin(OriginPlacement originPlacement)
        {
            switch (originPlacement)
            {
                case OriginPlacement.TopLeft:
                    return FrameTopLeftRelativeToSourceRect;
                case OriginPlacement.Center:
                    if (SourceCenterOrigin.HasValue) return SourceCenterOrigin.Value;
                    SourceCenterOrigin = FindCenterSourceOrigin();
                    return SourceCenterOrigin.Value;
                case OriginPlacement.BottomMiddle:
                    return new Vector2(FrameTopLeftRelativeToSourceRect.X + FrameWidth / 2, FrameTopLeftRelativeToSourceRect.Y + FrameHeight);
                case OriginPlacement.TopMiddle:
                    return new Vector2(FrameTopLeftRelativeToSourceRect.X + FrameWidth / 2, FrameTopLeftRelativeToSourceRect.Y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(originPlacement), originPlacement, "Invalid origin placement specified.");
            }
        }

        private Vector2 FindCenterSourceOrigin()
        {
            // Consider a source not centered within the frame
            // If the frame is in the bottom right, the center origin of the frame would be in that sources top left quadrant
            float leftSpace = -FrameTopLeftRelativeToSourceRect.X;
            float rightSpace = FrameWidth + FrameTopLeftRelativeToSourceRect.X - SourceRect.Width;
            float topSpace = -FrameTopLeftRelativeToSourceRect.Y;
            float bottomSpace = FrameHeight + FrameTopLeftRelativeToSourceRect.Y - SourceRect.Height;
            float horizDiff = (leftSpace - rightSpace) / 2;
            float vertDiff = (topSpace - bottomSpace) / 2;
            return new Vector2(SourceRect.Width / 2 - horizDiff, SourceRect.Height / 2 - vertDiff);
        }

        private Vector2? SourceCenterOrigin = null; 
    }

    public class AnimationFrameJsonConverter : JsonConverter<AnimationFrame>
    {
        public override AnimationFrame Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            int? sourceX = null;
            int? sourceY = null;
            int? originX = null;
            int? originY = null;
            int? frameWidth = null;
            int? frameHeight = null;
            int? sourceWidth = null;
            int? sourceHeight = null;
            string? name = null;

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

            if (originX == null) originX = 0;
            if (originY == null) originY = 0;
            if (frameWidth == null) frameWidth = sourceWidth;
            if (frameHeight == null) frameHeight = sourceHeight;

            if (!sourceX.HasValue || !sourceY.HasValue || !sourceWidth.HasValue || !sourceHeight.HasValue ||
                string.IsNullOrEmpty(name))
            {
                throw new JsonException("Missing required properties for AnimationFrame");
            }

            return new AnimationFrame(sourceX.Value, sourceY.Value, originX.Value, originY.Value, sourceWidth.Value, sourceHeight.Value, frameWidth.Value, frameHeight.Value, name);
        }

        public override void Write(Utf8JsonWriter writer, AnimationFrame value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
