using FileMcpServer.DataTransfer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMcpServer.Utility
{
    internal class FileConverter
    {
        /// <summary>
        /// This method returns the specified content as plain text or converts it to Markdown.
        /// </summary>
        /// <param name="contents">Original file contents.</param>
        /// <param name="format">The format of the original file.</param>
        /// <returns>The converted text.</returns>
        /// <exception cref="NotImplementedException">Format processing is not implemented.</exception>
        /// <exception cref="ArgumentException">Format is either Undefined or Unknown.</exception>
        internal static async Task<string> ConvertToTextAsync(string contents, FileFormat format)
        {
            switch (format)
            {
                case FileFormat.DOCX:
                    return await ConvertDocxToMarkdown(contents);
                case FileFormat.ODT:
                    throw new NotImplementedException("ODT to Markdown conversion is not implemented yet.");
                case FileFormat.PDF:
                    throw new NotImplementedException("PDF to Markdown conversion is not implemented yet.");
                case FileFormat.PlainText:
                case FileFormat.RichText:
                case FileFormat.Markdown:
                case FileFormat.HTML:
                    return contents;
                case FileFormat.Unknown:
                    throw new ArgumentException("Cannot convert unknown format to text.");
                case FileFormat.Undefined:
                    throw new ArgumentException("Cannot convert undefined format to text.");
                default:
                    throw new NotImplementedException($"Conversion from {format} to Markdown is not implemented yet.");
            }
        }

        public static async Task<string> ConvertDocxToMarkdown(string contents)
        {
            try
            {
                string pandocPath = GetExecutablePath("pandoc");

                // Create a copy of file contents to a temporary file.
                string tempInputFile = Path.GetTempFileName() + ".docx";
                await File.WriteAllBytesAsync(tempInputFile, Encoding.UTF8.GetBytes(contents));

                string tempOutputFile = Path.GetTempFileName() + ".md";
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = pandocPath,
                        Arguments = $"\"{tempInputFile}\" -f docx -t markdown -o \"{tempOutputFile}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                return File.ReadAllText(tempOutputFile);
            }
            catch (FileNotFoundException)
            {
                throw new InvalidOperationException("Pandoc is not installed or not found in system PATH.");
            }
            catch
            {
                throw new ApplicationException("An error occurred during DOCX to Markdown conversion.");
            }
        }

        public static string GetExecutablePath(string executableName)
        {
            // Determine full path of executable using "where" in Windows and "which" in Linux.
            string command = Environment.OSVersion.Platform == PlatformID.Win32NT ? "where" : "which";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = executableName,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                string commandOutput = process.StandardOutput.ReadToEnd();
                return GetFirstLine(commandOutput);
            }
            else
            {
                throw new FileNotFoundException($"{executableName} not found in system PATH.");
            }
        }

        private static string GetFirstLine(string commandOutput)
        {
            int newLineIndex = commandOutput.IndexOf(Environment.NewLine);
            return newLineIndex < 0 ? commandOutput : commandOutput.Remove(newLineIndex);
        }
    }
}
