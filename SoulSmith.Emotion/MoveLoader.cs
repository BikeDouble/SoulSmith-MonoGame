using SoulSmith.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Emotion
{
    public class EmotionLoader : IBasicAssetLoader
    {
        public IDisposable Load(string path)
        {
            if (!File.Exists(path)) return null;

            string fileText = File.ReadAllText(path);

            Emotion emotion = JsonSerializer.Deserialize<Emotion>(fileText);

            return emotion;
        }
    }
}
