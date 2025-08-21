using FileMcpServer.DataTransfer;
using FileMcpServer.Utility;
using System.Linq;

namespace FileMcpServer
{
    public partial class DragDropFileList : Form
    {
        internal ServerContext ServerContext { get; }

        internal DragDropFileList()
        {
            InitializeComponent();

            Program.ServerContext.AvailableFiles = new ListBoxItemsEnumerable<string>(listBoxPaths.Items)
                .AsQueryable().Cast<string>().Select(it => new FileContext(it));
        }
    }
}
