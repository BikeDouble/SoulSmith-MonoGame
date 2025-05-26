using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Core;

namespace SoulSmith.Drawing
{
    public interface IDrawableResource
    {
        public void Draw(IReadOnlyPosition position, Vector4 tint, SpriteBatch spriteBatch);
    }
}

