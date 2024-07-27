
using System;
using System.Diagnostics.Metrics;

//Handed out with asset to keep count of active asset instances
public partial class TrackedResource<T> where T : class
{
    private AssetCounter _counter;
    private T _resource;

    public TrackedResource(T resource, AssetCounter counter)
    {
        _resource = resource;
        _counter = counter;
        _counter.IncreaseCount();
    }

    public TrackedResource(TrackedResource<T> other)
    {
        _resource = other.Resource;
        _counter = other._counter;
        _counter.IncreaseCount();
    }

    ~TrackedResource()
    {
        _counter.DecreaseCount();
    }

    public static implicit operator T(TrackedResource<T> wrapper) { return wrapper.Resource; }
    public T Resource { get { return _resource; } }
}
