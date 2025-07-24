using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{
    public interface IGraphicsAssetLoader
    {
        public IDisposable Load(string path, GraphicsDevice graphics);
    }
}
