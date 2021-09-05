using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AgilityContXam
{
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
        {
            if (range == null) return;
            if (!range.Any()) return;

            foreach (T item in range)
                collection.Add(item);
        }
    }
}
