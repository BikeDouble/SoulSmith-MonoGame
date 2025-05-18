using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Drawing
{
    public interface IReadOnlyCanvasPosition
    {
        public Vector2 ScaleVector { get; }
        public float Width { get; }
        public float Height { get; }
        public Vector2 Coordinates { get; }
        public float Rotation { get; }
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
    }
}
