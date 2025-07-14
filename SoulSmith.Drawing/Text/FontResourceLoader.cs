using FontStashSharp;
using SoulSmith.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Text
{
    public class FontResourceLoader : IBasicAssetLoader
    {
        public IAsset Load(string path)
        {
            if (!File.Exists(path)) return null;

            FontSystem fontSystem = new FontSystem();
            fontSystem.AddFont(File.ReadAllBytes(path));

            return new FontResource(fontSystem);
        }
    }
}
