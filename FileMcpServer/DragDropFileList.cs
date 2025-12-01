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

            KeyPreview = true;

            ServerContext = context;
            ServerContext.AvailableFiles = new QueryableWrapper<FileContext>(
                () => ListBoxPaths.Items.Cast<string>().Select(it => new FileContext(it, PrintLengthObserver))
            );
        }

        private int PrintLengthObserver() => ListBoxPaths.Size.Width / 7;

        private void DragDropFileList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data!.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void DragDropFileList_DragDrop(object sender, DragEventArgs e)
        {
            var droppedItems = (string[])e.Data!.GetData(DataFormats.FileDrop)!;

            var allPaths = new List<string>();

            //// Recursively add files from dropped folders.
            //foreach (var path in droppedItems)
            //{
            //    if (Directory.Exists(path))
            //    {
            //        // Optionally, add all files in the folder recursively
            //        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            //        allPaths.AddRange(files);
            //    }
            //    else if (File.Exists(path))
            //    {
            //        allPaths.Add(path);
            //    }
            //}

            allPaths.AddRange(droppedItems);

            // Remove duplicates and add to ListBox
            foreach (var filePath in allPaths.Distinct())
            {
                if (!ListBoxPaths.Items.Contains(filePath))
                    ListBoxPaths.Items.Add(filePath);
            }
        }

        private void ListBoxPaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveButton.Enabled = ListBoxPaths.SelectedItems.Count > 0;
            RemoveAllButton.Enabled = ListBoxPaths.Items.Count > 0;
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            foreach (var item in ListBoxPaths.SelectedItems.Cast<string>().ToList())
                ListBoxPaths.Items.Remove(item);
        }

        private void RemoveAllButton_Click(object sender, EventArgs e)
        {
            ListBoxPaths.Items.Clear();
        }

        private void DragDropFileList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveButton.PerformClick();
            }
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
