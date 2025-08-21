namespace FileMcpServer.DataTransfer
{
    /// <summary>
    /// Identifies the content format based on the file extension. This determines how to expose
    /// the file content to the MCP client.
    /// </summary>
    internal enum FileFormat
    {
        Undefined,
        Unknown,
        PlainText,
        Markdown,
        HTML,
        DOCX,
        ODT,
        PDF
    }
}