
using System;
using System.Diagnostics.Metrics;

namespace SoulSmith.Asset
{
    //Handed out with asset to keep count of active asset instances
    public class TrackedAsset<T> : IReadOnlyTrackedAsset<T>
    {
        private AssetCounter _counter;
        private T _resource;

        public TrackedAsset(T resource, AssetCounter counter)
        {
            _resource = resource;
            _counter = counter;
            _counter.IncreaseCount();
        }

        public TrackedAsset(IReadOnlyTrackedAsset<T> other)
        {
            _resource = other.Value;
            _counter = other.Counter;
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
        public T Value { get { return _resource; } }
        public AssetCounter Counter { get { return _counter; } }
    }
}