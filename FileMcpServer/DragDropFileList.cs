using FileMcpServer.DataTransfer;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;

namespace FileMcpServer
{
    public partial class DragDropFileList : Form
    {
        ServerContext ServerContext { get; }
        BindingList<FileContext> AvailableFiles { get; } = new();

        internal DragDropFileList(ServerContext context)
        {
            InitializeComponent();

            KeyPreview = true;

            ServerContext = context;
            //ServerContext.AvailableFiles = new QueryableWrapper<FileContext>(
            //    ListBoxPaths.Items.Cast<FileContext>
            //);
            ServerContext.AvailableFiles = AvailableFiles.AsQueryable();

            ListBoxPaths.DataSource = AvailableFiles;
        }

        private int PrintLengthObserver() => ListBoxPaths.Size.Width / 6;

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
                if (!AvailableFiles.Any(it => it.FullPath == filePath))
                    AvailableFiles.Add(new FileContext(filePath, PrintLengthObserver));
            }
        }

        private void ListBoxPaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveButton.Enabled = ListBoxPaths.SelectedItems.Count > 0;
            RemoveAllButton.Enabled = AvailableFiles.Count > 0;
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            foreach (var item in ListBoxPaths.SelectedItems.Cast<FileContext>().ToList())
                AvailableFiles.Remove(item);
        }

        private void RemoveAllButton_Click(object sender, EventArgs e)
        {
            AvailableFiles.Clear();
        }

        private void DragDropFileList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveButton.PerformClick();
            }
        }

        private void ListBoxPaths_Resize(object sender, EventArgs e)
        {
            // Force the ListBox to update the displayed file paths.
            object? dataSource = ListBoxPaths.DataSource;
            ListBoxPaths.DataSource = null;
            ListBoxPaths.DataSource = dataSource;
        }
    }
}
