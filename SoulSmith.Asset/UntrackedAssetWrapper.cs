using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{
    public class UntrackedAssetWrapper<T> : IAssetWrapper<T> where T : IDisposable
    {
        private T _value;

        public UntrackedAssetWrapper(T value) 
        {
            _value = value;
        }

        public void Dispose()
        {
            _value?.Dispose();
        }

        public T Value { get { return _value; } }
    }
}
