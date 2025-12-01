
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
            // Combine whether file or folder and then the full path.
            string fileDetails = (IsFolder ? "[Folder] " : "[File] ") + FullPath;

            // Displays first 20 characters from the beginning, then ... and last printLength - 23 characters.
            if (fileDetails.Length <= PrintLength)
                return fileDetails;
            else
                return fileDetails.Substring(0, 20) + "..." + fileDetails.Substring(fileDetails.Length - (PrintLength - 23));
        }
    }
}