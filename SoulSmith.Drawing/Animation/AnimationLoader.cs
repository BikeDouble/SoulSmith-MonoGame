using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Animation
{
    public class AnimationLoader : IBasicAssetLoader
    {
        public IAsset Load(string path)
        {
            if (!File.Exists(path)) return null;

            Animation data = JsonSerializer.Deserialize<Animation>(File.ReadAllText(path));

            return data;
        }
    }
}
