using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{
    public interface IAssetWrapper<out T> : System.IDisposable where T : IDisposable
    {
        T Value { get; }
    }
}
