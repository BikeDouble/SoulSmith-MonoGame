
using System;

public partial class AssetCounter 
{
    private int _count;
    
    public AssetCounter() { _count = 0; }

    public void IncreaseCount()
    {
        _count++;
    }

    public void DecreaseCount()
    {
        _count--;
    }

    public int Count { get { return _count; } }
}
