using Microsoft.Xna.Framework.Graphics;
using SoulSmith.Asset;
using SoulSmith.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoulSmith.Battle.Effects.Visualization.Factory
{
    public class EffectVisualizationFactoryLoader : IBasicAssetLoader
    {
        public IAsset Load(string path)
        {
            if (!File.Exists(path)) return null;

            EffectVisualizationFactory resource = JsonSerializer.Deserialize<EffectVisualizationFactory>(File.ReadAllText(path));

            return resource;
        }
    }
}
