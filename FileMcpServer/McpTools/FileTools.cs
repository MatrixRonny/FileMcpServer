using FileMcpServer.DataTransfer;
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

            return files.Select(file => file.FullPath);
        }

        [McpServerTool(Title = "Read entire text file and return is as Markdown.", ReadOnly = true)]
        public static async Task<string> ReadFileAsync(
            [Description("Absolute path under one of the allowed roots")] string filePath,
            CancellationToken token)
        {
            //TODO: Convert non txt or Markdown files to Markdown.

            ServerContext context = Program.Services.GetRequiredService<ServerContext>();
            if (context.AvailableFiles == null)
            {
                throw new InvalidOperationException("ServerContext is not initialized.");
            }

            var files = ExpandFolderFiles(context.AvailableFiles);
            FileContext? file = files.SingleOrDefault(file => String.Equals(filePath, file.FullPath, StringComparison.OrdinalIgnoreCase));

            if (file == null)
                throw new FileNotFoundException($"File '{filePath}' not found on the server.");

            string content = await File.ReadAllTextAsync(file.FullPath, Encoding.UTF8, token);
            return await ConvertDocumentToMarkdownAsync(content, file.FileType);
        }

        [McpServerTool(Title = "Read entire text file and return is as Markdown.", ReadOnly = false)]
        public static async Task WriteFileAsync(
            [Description("Absolute path for file exposed by server.")] string filePath,
            [Description("Plain text or Markdown to overwrite the file.")] string content,
            CancellationToken token)
        {
            ServerContext context = Program.Services.GetRequiredService<ServerContext>();
            if (context.AvailableFiles == null)
            {
                throw new InvalidOperationException("ServerContext is not initialized.");
            }

            FileContext? file = context.AvailableFiles.SingleOrDefault(file => String.Equals(filePath, file.FullPath, StringComparison.OrdinalIgnoreCase));
            if (file == null)
                throw new FileNotFoundException($"File '{filePath}' not found on the server.");

            content = await ConvertMarkdownToDocumentAsync(content, file.FileType);
            await File.WriteAllTextAsync(file.FullPath, content, Encoding.UTF8, token);
        }

        #region Helper methods

        private static IEnumerable<FileContext> ExpandFolderFiles(IQueryable<FileContext> availableFiles)
        {
            var filesInFolders = availableFiles
                .Where(file => file.IsFolder)
                .SelectMany(folder => Directory.GetFiles(folder.FullPath))
                .Select(filePath => new FileContext(filePath));
            return availableFiles.Union(filesInFolders);
        }

        /// <summary>
        /// Converts non-text files to Markdown format, for example DOCX, ODT or PDF files.
        /// </summary>
        private static async Task<string> ConvertDocumentToMarkdownAsync(string contents, FileFormat format)
        {
            //TODO: Check if the file is already in Markdown format.
            //TODO: Implement conversion from document to Markdown.
            await Task.CompletedTask;

            return contents;
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

        #endregion
    }
}
