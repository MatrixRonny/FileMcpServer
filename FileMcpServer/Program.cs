using FileMcpServer.DataTransfer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileMcpServer
{
    internal static class Program
    {
        public static IHost Host { get; }
        public static IServiceProvider Services { get; }

        static Program()
        {
            Host = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .ConfigureServices((ctx, svc) =>
                {
                    svc.AddMcpServer()
                       .WithStdioServerTransport()  // easiest desktop transport
                       .WithToolsFromAssembly();    // will auto-discover our tools

                    // Register any additional services here
                    svc.AddSingleton<ServerContext>();
                })
                .Build();

            Services = Host.Services; // make the service provider available to the rest of the app
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _ = Host.RunAsync(); // fire-and-forget

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            ServerContext context = Services.GetRequiredService<ServerContext>();
            Application.Run(new DragDropFileList(context));
        }
    }
}