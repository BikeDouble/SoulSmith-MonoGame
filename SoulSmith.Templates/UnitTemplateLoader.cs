using SoulSmith.Asset;
using System.Text.Json;

namespace SoulSmith.Templates
{
    public class UnitTemplateLoader : IBasicAssetLoader
    {
        public IAsset Load(string path)
        {
            if (!File.Exists(path)) return null;

            string fileText = File.ReadAllText(path);

            UnitTemplate template = JsonSerializer.Deserialize<UnitTemplate>(fileText);

            return template;
        }
    }
}
