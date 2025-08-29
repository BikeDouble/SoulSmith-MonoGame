using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Core
{
    [JsonConverter(typeof(KeyPathManifestJsonConverter))]
    public class KeyPathManifest
    {
        public Dictionary<string, string> Manifest;

        public KeyPathManifest(IDictionary<string, string> manifest) 
        {
            Manifest = new Dictionary<string, string>(manifest);
        }
    }

    internal class KeyPathManifestJsonConverter : System.Text.Json.Serialization.JsonConverter<KeyPathManifest> //TODO test!
    {
        public override KeyPathManifest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start of object");
            reader.Read();
            string text = reader.GetString() ?? throw new JsonException("String cannot be null");
            Stack<string> lastAddedNames = new Stack<string>();
            lastAddedNames.Push(text);
            Dictionary<string, string> dict = new();
            reader.Read();

            while (!((lastAddedNames.Count() == 0) && (reader.TokenType == JsonTokenType.EndObject)))
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string newText = reader.GetString() ?? throw new JsonException("String cannot be null");
                    text = AddToPath(text, newText);
                    lastAddedNames.Push(newText);
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    text = RemoveFromPath(text, lastAddedNames.Pop());
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    dict.Add(text, reader.GetString() ?? throw new JsonException("String cannot be null"));
                    text = RemoveFromPath(text, lastAddedNames.Pop());
                }
                reader.Read();
            }

            return new KeyPathManifest(dict);
        }

        private string AddToPath(string predecessors, string name)
        {
            if (predecessors.Length == 0) return name;

            return predecessors + "/" + name;
        }

        private string RemoveFromPath(string predecessors, string name)
        {
            if (predecessors.Length <= name.Length) return string.Empty;

            return predecessors.Remove(predecessors.Length - name.Length - 1);
        }

        public override void Write(Utf8JsonWriter writer, KeyPathManifest value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
