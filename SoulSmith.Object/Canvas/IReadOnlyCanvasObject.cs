using Microsoft.Xna.Framework;
using SoulSmith.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SoulSmith.Object.Canvas;
public interface IReadOnlyCanvasObject : IReadOnlySoulSmithObject
{
    bool ContainsPointRelative(Vector2 point);
    Position GetGlobalPosition();
}