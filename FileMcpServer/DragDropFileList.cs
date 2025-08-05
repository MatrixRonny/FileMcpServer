using FileMcpServer.DataTransfer;
using System.Linq;

namespace FileMcpServer
{
    public partial class DragDropFileList : Form
    {
        internal ServerContext ServerContext { get; }

        internal DragDropFileList(ServerContext context)
        {
            InitializeComponent();

            ServerContext = context;
            ServerContext.AvailableFiles = listBoxPaths.Items.AsQueryable().Cast<string>().Select(it => new FileContext(it));
        }
    }
}
