
namespace FileMcpServer.DataTransfer
{
    internal class FileContext
    {
        public string FullPath { get; init; }
        public string? FileName => Path.GetFileName(FullPath);
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
                    ".docx" => FileFormat.DOCX,
                    ".odt" => FileFormat.ODT,
                    ".pdf" => FileFormat.PDF,
                    ".html" => FileFormat.HTML,
                    _ => FileFormat.Unknown
                };
            }
        }

        public FileContext(string filePath)
        {
            FullPath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }
}