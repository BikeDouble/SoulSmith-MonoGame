using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Collections
{
    public interface IReadOnlySoulSmithWeightedList<T> : IEnumerable<T>
    {
        public List<T> NextMultiple(int amount, bool allowDuplicates = false);
        public T Next();
        public int Count { get; }
    }
}
