using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMcpServer.DataTransfer
{
    internal class ServerContext
    {
        public IQueryable<FileContext>? AvailableFiles; // This IQueryable exposes the list of files and folders exposed by the server.
    }
}
