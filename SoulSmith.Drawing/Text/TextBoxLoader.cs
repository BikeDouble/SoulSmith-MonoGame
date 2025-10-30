using SoulSmith.Asset;
using System.Text.Json;

namespace SoulSmith.Drawing.Text
{
    public class TextBoxLoader : IBasicAssetLoader
    {
        public IDisposable Load(string path)
        {
            if (!File.Exists(path)) return null;

            TextBox box = JsonSerializer.Deserialize<TextBox>(File.ReadAllText(path));

            if (box == null) throw new ArgumentNullException(nameof(box));

            return box;
        }
    }
}
