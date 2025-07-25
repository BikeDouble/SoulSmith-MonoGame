using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SoulSmith.Core
{
    public interface IReadOnlyPosition
    {
        public Vector2 ScaleVector { get; }
        public float Width { get; }
        public float Height { get; }
        public Vector2 Coordinates { get; }
        public float Rotation { get; }
        public float X { get; }
        public float Y { get; }
        public int Z { get; }
    }
}
