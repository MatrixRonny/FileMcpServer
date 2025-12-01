using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FileMcpServer.Utility
{
    internal class QueryableWrapper<T> : IQueryable<T>
    {
        private readonly IQueryable<T> _queryable;

        public QueryableWrapper(Func<IEnumerable<T>> itemsQuery)
        {
            _queryable = itemsQuery().AsQueryable();
        }

        public Type ElementType => _queryable.ElementType;

        public Expression Expression => _queryable.Expression;

        public IQueryProvider Provider => _queryable.Provider;

        public IEnumerator<T> GetEnumerator() => _queryable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queryable.GetEnumerator();
    }
}
