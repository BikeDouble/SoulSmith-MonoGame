
using System;
using System.Diagnostics.Metrics;

namespace SoulSmith.Asset
{
    //Handed out with asset to keep count of active asset instances
    public partial class TrackedAsset<T> : IDisposable where T : class
    {
        private AssetCounter _counter;
        private T _resource;

        public TrackedAsset(T resource, AssetCounter counter)
        {
            _resource = resource;
            _counter = counter;
            _counter.IncreaseCount();
        }

        public TrackedAsset(TrackedAsset<T> other)
        {
            _resource = other.Resource;
            _counter = other._counter;
            _counter.IncreaseCount();
        }

        public void Dispose() //TODO add disposing properly!
        {
            _counter.DecreaseCount();
            GC.SuppressFinalize(this);
        }

        ~TrackedAsset()
        {
            _counter.DecreaseCount();
        }


        //public static implicit operator T(TrackedAsset<T> wrapper) { return wrapper?.Resource; }
        public T Resource { get { return _resource; } }
    }
}