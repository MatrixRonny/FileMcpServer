using FileMcpServer.DataTransfer;

namespace FileMcpServer
{
    internal static class Program
    {
        public static readonly ServerContext ServerContext = new ServerContext();

        async static void StartWebServer()
        {
            IHost Host = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel()
                        .UseUrls("http://localhost:5000") // Set the URL for the web server
                        .UseStartup<KestrelStartup>(); // Specify the Startup class for configuration

                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddMcpServer()
                           .WithStdioServerTransport()  // easiest desktop transport
                           .WithToolsFromAssembly();    // will auto-discover our tools

                        // Register any additional services here
                        services.AddSingleton<ServerContext>();
                    });
                })
                .Build();

            //INFO: Assigning Host.Services here does not work as application is not yet started.

            await Host.RunAsync(); // fire-and-forget
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            StartWebServer();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Application.Run(new DragDropFileList());
        }
    }

    internal class KestrelStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Use this method to add services to the container.

            //services.AddControllers(); // Also uncomment MapControllers in Configure method.
            services.AddSingleton(Program.ServerContext);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Use this method to configure the HTTP request pipeline.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to the File MCP Server!");
                });
            });
        }
    }
}