using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{
    internal class CachedAsset<T> : IDisposable
    {
        private T _asset;
        private AssetCounter _counter;

        public CachedAsset(T asset)
        {
            _asset = asset;
            _counter = new AssetCounter();
        }

        public void Dispose()
        {

        }

        public IReadOnlyTrackedAsset<T> GetAsset()
        {
            return new TrackedAsset<T>(_asset, _counter);
        }

        public int ReferenceCount { get { return _counter.Count; } }
        public bool HasNoReferences { get { return _counter.Count <= 0;} }
    }
}
