using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViennaDotNet.Common.Utils
{
    public static class LinkedListExtensions
    {
        public static void AddRange<T>(this LinkedList<T> list, IEnumerable<T> other)
        {
            foreach (T item in other)
                list.AddLast(item);
        }
    }
}
