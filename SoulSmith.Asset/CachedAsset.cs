using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{
    internal class CachedAsset : System.IDisposable
    {
        private IDisposable _asset;
        private AssetCounter _counter;

        public CachedAsset(IDisposable asset)
        {
            _asset = asset;
            _counter = new AssetCounter();
        }

        public void Dispose()
        {

        }

        public IReadOnlyTrackedAsset<T> GetTrackedIAsset<T>() where T : IDisposable
        {
            T assetAsT = (T)_asset;
            return new TrackedAsset<T>(assetAsT, _counter);
        }

        public int ReferenceCount { get { return _counter.Count; } }
        public bool HasNoReferences { get { return _counter.Count <= 0;} }
    }
}
