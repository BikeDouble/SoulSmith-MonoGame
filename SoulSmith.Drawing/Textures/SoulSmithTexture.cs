using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Drawing.Textures
{
    public class SoulSmithTexture : IDisposable
    {
        public SoulSmithTexture(Texture2D texture, SoulSmithTextureMetaData metaData = null) 
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture), "Texture cannot be null.");
            SamplerState = metaData?.SamplerState ?? SamplerState.PointClamp;
        }

        public void Dispose()
        {
            Texture?.Dispose();
            Texture = null;
        }

        public Texture2D Texture { get; private set; }
        public SamplerState SamplerState { get; private set; }
        public int Width => Texture?.Width ?? 0;
        public int Height => Texture?.Height ?? 0;
    }
}
