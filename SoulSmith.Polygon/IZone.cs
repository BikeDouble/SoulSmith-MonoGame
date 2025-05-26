using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulSmith.Core;


namespace SoulSmith.Shapes
{
    public interface IZone
    {
        public bool Contains(Vector2 point);
    }
}
