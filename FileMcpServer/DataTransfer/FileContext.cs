namespace FileMcpServer.DataTransfer
{
    internal class FileContext
    {
        public string FilePath { get; init; }
        public string FileName => Path.GetFileName(FilePath);
        public string FileExtension => Path.GetExtension(FileName);
        public bool IsFolder => Directory.Exists(FilePath);

        public FileContext(string filePath)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }
}