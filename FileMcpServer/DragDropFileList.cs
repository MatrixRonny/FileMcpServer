using FileMcpServer.DataTransfer;
using System.Collections;
using System.Linq.Expressions;

namespace FileMcpServer
{
    public partial class DragDropFileList : Form
    {
        internal ServerContext ServerContext { get; }

        internal DragDropFileList(ServerContext context)
        {
            InitializeComponent();

            ServerContext = context;
            ServerContext.AvailableFiles = new QueryableWrapper<FileContext>(
                () => listBoxPaths.Items.Cast<string>().Select(it => new FileContext(it))
            );
        }
    }

    class QueryableWrapper<T> : IQueryable<T>
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
