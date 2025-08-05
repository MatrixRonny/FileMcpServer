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

            return files.Select(file => file.FilePath);
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
            FileContext? file = files.SingleOrDefault(file => String.Equals(filePath, file.FilePath, StringComparison.OrdinalIgnoreCase));

            if (file == null)
                throw new FileNotFoundException($"File '{filePath}' not found on the server.");

            return await File.ReadAllTextAsync(file.FilePath, Encoding.UTF8, token);
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

            FileContext? file = context.AvailableFiles.SingleOrDefault(file => String.Equals(filePath, file.FilePath, StringComparison.OrdinalIgnoreCase));
            if (file == null)
                throw new FileNotFoundException($"File '{filePath}' not found on the server.");

            await File.WriteAllTextAsync(file.FilePath, content, Encoding.UTF8, token);
        }

        #region Helper methods

        private static IEnumerable<FileContext> ExpandFolderFiles(IQueryable<FileContext> availableFiles)
        {
            var filesInFolders = availableFiles
                .Where(file => file.IsFolder)
                .SelectMany(folder => Directory.GetFiles(folder.FilePath))
                .Select(filePath => new FileContext(filePath));
            return availableFiles.Union(filesInFolders);
        }

        private static Task<string> ConvertDocumentToMarkdownAsync(string contents)
        {
            throw new NotImplementedException();
        }

        private static Task<string> ConvertMarkdownToDocumentAsync(string contents)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
