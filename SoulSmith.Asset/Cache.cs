using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Asset
{
    internal class Cache<T> 
    {
        Dictionary<string, CachedAsset<T>> _cache = new Dictionary<string, CachedAsset<T>>();

        public Cache() 
        {

        }


        /// <summary>
        /// Removes all assets with no references.
        /// </summary>
        /// <returns>Number of assets removed.</returns>
        public int Clean()
        {
            List<string> toRemove = new List<string>();

            foreach (var item in _cache) 
            {
                if (item.Value.HasNoReferences)
                {
                    toRemove.Add(item.Key);
                }
            }

            foreach (string key in toRemove)
            {
                _cache.Remove(key);
            }

            return toRemove.Count;
        }

        public void CacheAsset(string name, T asset)
        {
            if (_cache.ContainsKey(name)) return;

            _cache.Add(name, new CachedAsset<T>(asset));
        }

        public bool Contains(string key)
        {
            return _cache.ContainsKey(key);
        }

        public TrackedAsset<T> GetAsset(string key)
        {
            return _cache[key]?.GetAsset();
        }        
    }
}
