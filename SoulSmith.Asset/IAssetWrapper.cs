using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{
    public interface IAssetWrapper<out T> : IDisposable where T : IAsset
    {
        T Value { get; }
    }
}
