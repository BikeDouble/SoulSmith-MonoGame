using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulSmith.Core
{
    public interface IAddOnly<T>
    {
        void Add(T item);
    }
}
