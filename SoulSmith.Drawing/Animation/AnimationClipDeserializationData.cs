using SoulSmith.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Animation
{
    [JsonConverter(typeof(AnimationClipDeserializationDataJsonConverter))]
    internal class AnimationClipDeserializationData
    {
        public AnimationClipDeserializationData(string clipFriendlyName, string frameDataName, int[] transitionFrames) 
        {
            ClipFriendlyName = clipFriendlyName;
            FrameDataName = frameDataName;
            TransitionFrames = transitionFrames;
        }

        public string ClipFriendlyName;
        public string FrameDataName;
        public int[] TransitionFrames;
    }

    internal class AnimationClipDeserializationDataJsonConverter : JsonConverter<AnimationClipDeserializationData>
    {
        public override AnimationClipDeserializationData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of an object");

            reader.Read();

            string clipFriendlyName = string.Empty;
            string frameDataName = string.Empty;
            int[] transitionFrames = null;

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected property name");

                string propertyName = reader.GetString();

                reader.Read();

                switch (propertyName)
                {
                    case "ClipFriendlyName":
                        clipFriendlyName = reader.GetString();
                        reader.Read();
                        break;
                    case "FrameDataName":
                        frameDataName = reader.GetString();
                        reader.Read();
                        break;
                    case "TransitionFrames":
                        transitionFrames = JsonSerializer.Deserialize<int[]>(ref reader, options);
                        reader.Read();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return new AnimationClipDeserializationData(clipFriendlyName, frameDataName, transitionFrames);
        }

        public override void Write(Utf8JsonWriter writer, AnimationClipDeserializationData value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
