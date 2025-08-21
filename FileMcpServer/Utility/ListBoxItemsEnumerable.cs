using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMcpServer.Utility
{
    public class ListBoxItemsEnumerable<T> : IEnumerable<T>
    {
        private readonly ListBox.ObjectCollection _items;

        public ListBoxItemsEnumerable(ListBox.ObjectCollection items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _items)
            {
                if (item is T validItem)
                    yield return validItem;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
