
using System.Diagnostics.CodeAnalysis;

namespace FileMcpServer.DataTransfer
{
    internal class FileContext
    {
        public required string FullPath { get; init; }
        public string FileName => Path.GetFileName(FullPath);
        public bool IsFolder => Directory.Exists(FullPath);

        public string? FileExtension => Path.GetExtension(FileName);
        public FileFormat FileType
        {
            get
            {
                return FileExtension switch
                {
                    ".txt" => FileFormat.PlainText,
                    ".md" => FileFormat.Markdown,
                    ".rtf" => FileFormat.RichText,
                    ".docx" => FileFormat.DOCX,
                    ".odt" => FileFormat.ODT,
                    ".pdf" => FileFormat.PDF,
                    ".html" => FileFormat.HTML,
                    _ => FileFormat.Unknown
                };
            }
        }

        public int PrintLength => PrintLengthObserver();
        public required Func<int> PrintLengthObserver { get; init; }

        public FileContext() { }

        [SetsRequiredMembers]
        public FileContext(string filePath, Func<int> printLengthObserver)
        {
            FullPath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            PrintLengthObserver = printLengthObserver ?? throw new ArgumentNullException(nameof(printLengthObserver));
        }

        public override string ToString()
        {
            // Displays first 10 characters from the beginning, then ... and last printLength - 13 characters.
            if (FullPath.Length <= PrintLength)
                return FullPath;
            else
                return FullPath.Substring(0, 10) + "..." + FullPath.Substring(FullPath.Length - (PrintLength - 13));
        }
    }
}