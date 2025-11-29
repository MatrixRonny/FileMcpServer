using FileMcpServer.DataTransfer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace FileMcpServer
{
    internal static class Program
    {
        /// <summary>
        /// FileTools class cannot be converted to singleton service because of limitations with MCP server library.
        /// This property is assigned before the GUI component starts and should only be used there.
        /// Web server code should access required services via dependency injection.
        /// </summary>
        public static IServiceProvider Services { get; private set; } = null!;

        static IHost CreateAndConfigureAppHost()
        {
            // Add MCP server with ASP.NET Core transport
            var builder = WebApplication.CreateBuilder();

            // Set Kestrel to listen on a specific port (e.g., 5005)
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5123);
            });

            builder.Services.AddMcpServer()
                .WithHttpTransport()
                .WithToolsFromAssembly();

            // Register any additional services here
            builder.Services
                .AddSingleton<ServerContext>()
                .AddHostedService<GuiHostedService>();

            var app = builder.Build();

            app.MapMcp("mcp");

            return app;
        }

        static void Main()
        {
            IHost appHost = CreateAndConfigureAppHost();

            // Start the host and wait.
            appHost.Run();
        }

        class GuiHostedService : IHostedService
        {
            private Thread _thread;

            public GuiHostedService(IServiceProvider services)
            {
                Services = services;
                ServerContext context = services.GetRequiredService<ServerContext>();

                _thread = new Thread(() =>
                {
                    ApplicationConfiguration.Initialize();

                    //Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new DragDropFileList(context));
                });
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Start();

                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                if (_thread != null && _thread.IsAlive)
                {
                    Application.ExitThread();
                    _thread.Join();
                }

                return Task.CompletedTask;
            }
        }

    }
}