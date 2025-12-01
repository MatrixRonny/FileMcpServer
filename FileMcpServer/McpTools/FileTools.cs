using FileMcpServer.DataTransfer;
using FileMcpServer.Utility;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMcpServer.McpTools
{
    [McpServerToolType]
    public static class FileTools
    {
        [McpServerTool(Title = "Get list of files available on the server.", ReadOnly = true)]
        public static IEnumerable<string> GetAvailableFiles(
            [Description("Optional filter to apply to the file list.")] string? filter = null,
            CancellationToken token = default)
        {
            ServerContext context = Program.Services.GetRequiredService<ServerContext>();
            if (context.AvailableFiles == null)
            {
                throw new InvalidOperationException("ServerContext is not initialized.");
            }

            // Apply filter if provided
            var files = ExpandFolderFiles(context.AvailableFiles);
            if (!string.IsNullOrEmpty(filter))
                files = files.Where(file => file.FileName.Contains(filter, StringComparison.OrdinalIgnoreCase));

            // Convert paths to use forward slashes and remove colons from drive letters.
            return files.Select(file => file.FullPath).Select(WindowsToUnixPath).Distinct();
        }

        [McpServerTool(Title = "Read entire text file and return is as Markdown.", ReadOnly = true)]
        public static async Task<string> ReadFileAsync(
            [Description("Absolute path under one of the allowed roots")] string filePath,
            CancellationToken token)
        {
            try
            {
                //TODO: Convert non txt or Markdown files to Markdown.

                ServerContext context = Program.Services.GetRequiredService<ServerContext>();
                if (context.AvailableFiles == null)
                {
                    throw new InvalidOperationException("ServerContext is not initialized.");
                }

                filePath = UnixToWindowsPath(filePath);

                var files = ExpandFolderFiles(context.AvailableFiles);
                FileContext? file = files.SingleOrDefault(file => String.Equals(filePath, file.FullPath, StringComparison.OrdinalIgnoreCase));

                if (file == null)
                    throw new FileNotFoundException($"File '{WindowsToUnixPath(filePath)}' not found on the server.");

                string content = await File.ReadAllTextAsync(file.FullPath, Encoding.UTF8, token);
                return await ConvertDocumentToMarkdownAsync(content, file.FileType);
            }
            catch (Exception ex)
            {
                return $"Error reading file: {ex.Message}";
            }
        }

        [McpServerTool(Title = "Read entire text file and return is as Markdown.", ReadOnly = false)]
        public static async Task<string> WriteFileAsync(
            [Description("Absolute path for file exposed by server.")] string filePath,
            [Description("Plain text or Markdown to overwrite the file.")] string content,
            CancellationToken token)
        {
            try
            {
                ServerContext context = Program.Services.GetRequiredService<ServerContext>();
                if (context.AvailableFiles == null)
                {
                    throw new InvalidOperationException("ServerContext is not initialized.");
                }

                filePath = UnixToWindowsPath(filePath);

                FileContext? file = context.AvailableFiles.SingleOrDefault(file => String.Equals(filePath, file.FullPath, StringComparison.OrdinalIgnoreCase));
                if (file == null)
                    throw new FileNotFoundException($"File '{WindowsToUnixPath(filePath)}' not found on the server.");

                content = await ConvertMarkdownToDocumentAsync(content, file.FileType);
                await File.WriteAllTextAsync(file.FullPath, content, Encoding.UTF8, token);

                return "File written successfully.";
            }
            catch (Exception ex)
            {
                return $"Error writing file: {ex.Message}";
            }
        }

        #region Helper methods

        private static IEnumerable<FileContext> ExpandFolderFiles(IQueryable<FileContext> availableFiles)
        {
            FileContext? fileOrFolder = availableFiles.FirstOrDefault();
            if (fileOrFolder == null)
                return Enumerable.Empty<FileContext>();

            Func<int> PrintLengthObserver = fileOrFolder.PrintLengthObserver;

            List<FileContext> allFiles = availableFiles.Where(f => !f.IsFolder).ToList();
            Queue<FileContext> foldersOnly = new(availableFiles.Where(f => f.IsFolder));

            while (foldersOnly.Any())
            {
                var folder = foldersOnly.Dequeue();

                var folderFiles = Directory.GetFiles(folder.FullPath).Select(filePath => new FileContext
                {
                    FullPath = filePath,
                    PrintLengthObserver = PrintLengthObserver
                });
                var foldersInFolder = Directory.GetDirectories(folder.FullPath).Select(dirPath => new FileContext
                {
                    FullPath = dirPath,
                    PrintLengthObserver = PrintLengthObserver
                });

                foreach (var innerFolder in foldersInFolder)
                {
                    foldersOnly.Enqueue(innerFolder);
                }

                allFiles.AddRange(folderFiles);
            }

            return allFiles;
        }

        /// <summary>
        /// Converts non-text files to Markdown format, for example DOCX, ODT or PDF files.
        /// </summary>
        private static async Task<string> ConvertDocumentToMarkdownAsync(string contents, FileFormat format)
        {
            try
            {
                return await FileConverter.ConvertToTextAsync(contents, format);
            }
            catch (Exception ex)
            {
                return $"Error converting \"{format}\" format to text: {ex.Message}";
            }
        }

        /// <summary>
        /// Converts the provided Markdown content into a document formatted according to the specified file format.
        /// </summary>
        private static async Task<string> ConvertMarkdownToDocumentAsync(string contents, FileFormat format)
        {
            //TODO: Check if conversion is necessary.
            //TODO: Implement conversion from Markdown to document.
            await Task.CompletedTask;

            return contents;
        }

        private static string WindowsToUnixPath(string filePath)
        {
            return filePath[0] + filePath.Substring(2).Replace('\\', '/');
            //return filePath.Replace('\\', '/');
        }

        private static string UnixToWindowsPath(string filePath)
        {
            bool hasColon = filePath[1] == ':';

            return filePath[0] + ":" + filePath.Substring(hasColon ? 2 : 1).Replace('/', '\\');
            //return filePath.Replace('/', '\\');
        }

        #endregion
    }
}
