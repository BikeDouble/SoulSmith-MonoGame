using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Core;
using SoulSmith.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Animation
{
    [JsonConverter(typeof(AnimationDataJsonConverter))]
    public class Animation : IAsset, IDisposable
    {
        public const int FRAMESPERSECOND = 60;

        private IDictionary<string, AnimationClip> _clips;
        private IAssetWrapper<Texture2DResource> _wrappedTexture;

        public Animation(IDictionary<string, AnimationClip> clips, IAssetWrapper<Texture2DResource> wrappedTexture)
        {
            _clips = clips;
            _wrappedTexture = wrappedTexture;
        }

        public void DrawFrame(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch, string clipName, double timeInClip, double animationSpeed = 1d)
        {
            AnimationClip activeClip = _clips[clipName];

            if (activeClip == null) activeClip = _clips.FirstOrDefault().Value;

            activeClip.DrawFrame(position, tint, spriteBatch, _wrappedTexture, timeInClip, animationSpeed);
        }

        private AnimationClip GetDefaultClip()
        {
            return _clips.FirstOrDefault().Value;
        }

        public string GetDefaultClipName()
        {
            return _clips.FirstOrDefault().Key;
        }

        public bool ContainsClip(string clipName)
        {
            return _clips.ContainsKey(clipName);
        }

        public void Dispose()
        {
            _clips.Clear();
            _wrappedTexture.Dispose();
        }

        public int Height { get { return GetDefaultClip().Height; } }
        public int Width { get { return GetDefaultClip().Width; } }
        public Vector2 Origin { get { return GetDefaultClip().Origin; } }
    }

    public class AnimationDataJsonConverter : JsonConverter<Animation>
    {
        public override Animation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            IAssetWrapper<Texture2DResource> wrappedTexture = null;
            (string ClipFriendlyName, string FrameDataName, int[] TransitionFrames)[] clipsData = null;
            AnimationFrame[] frames = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "Texture":
                    case "TextureKey":
                        string textureKey = reader.GetString();
                        wrappedTexture = AssetManager.Instance.GetZonedResource<Texture2DResource>(textureKey);
                        reader.Read();
                        break;
                    case "ClipData":
                    case "ClipsData":
                        clipsData = JsonSerializer.Deserialize<(string ClipFriendlyName, string FrameDataName, int[] TransitionFrames)[]>(ref reader, options);
                        reader.Read();
                        break;
                    case "FrameData":
                    case "Frames":
                        frames = JsonSerializer.Deserialize<AnimationFrame[]>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            if (wrappedTexture == null) throw new ArgumentNullException(nameof(wrappedTexture));
            if (frames == null) throw new ArgumentNullException(nameof(frames));
            if (clipsData == null) throw new ArgumentNullException(nameof(clipsData));

            Dictionary<string, AnimationClip> clipsDict = new();

            foreach ((string ClipFriendlyName, string FrameDataName, int[] TransitionFrames) clipData in clipsData)
            {
                AnimationClip clip = new AnimationClip(frames, clipData.FrameDataName, clipData.TransitionFrames);
                clipsDict.Add(clipData.ClipFriendlyName, clip);
            }

            return new Animation(clipsDict, wrappedTexture);
        }

        public override void Write(Utf8JsonWriter writer, Animation value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
