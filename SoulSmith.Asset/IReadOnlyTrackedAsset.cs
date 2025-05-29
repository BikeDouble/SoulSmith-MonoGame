using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{
    public interface IReadOnlyTrackedAsset<out T> : IDisposable
    {
        T Value { get; }
        AssetCounter Counter { get; }
    }
}
