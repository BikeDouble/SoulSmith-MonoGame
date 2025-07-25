using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SoulSmith.Drawing
{
    [JsonConverter(typeof(DrawableResourceKeyJsonConverter))]
    public class DrawableResourceKey
    {
        public string Key { get; }
        public string Type { get; }
        public DrawableResourceKey(string key, string type)
        {
            Key = key;
            Type = type;
        }
    }


}
