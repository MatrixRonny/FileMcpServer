using FileMcpServer.DataTransfer;

#pragma warning disable IDE0051  // Private member is unused.

namespace FileMcpServer
{
    internal static class Program
    {
        public static readonly ServerContext ServerContext = new ServerContext();

        async static void StartWebServerV1()
        {
            var app = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Configure Kestrel server options
                    webBuilder.UseKestrel()
                        .UseUrls("http://localhost:5000") // Set the URL for the web server
                        .UseStartup<KestrelStartup>(); // Specify the Startup class for configuration

                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddMcpServer()
                           //.WithStdioServerTransport()  // best for local MCP host
                           .WithHttpTransport() // best for remote MCP host
                           .WithToolsFromAssembly();    // will auto-discover our tools

                        // Register any additional services here
                        services.AddSingleton<ServerContext>();
                    });
                })
                .Build();

            //INFO: Assigning Host.Services here does not work as application is not yet started.

            await app.RunAsync(); // fire-and-forget
        }

        async static void StartWebServerV2()
        {
            var builder = WebApplication.CreateBuilder();

            builder.WebHost.UseKestrel()
                .UseUrls("http://localhost:5000"); // Set the URL for the web server

            IServiceCollection services = builder.Services;

            builder.Services.AddMcpServer()
                //.WithStdioServerTransport()  // best for local MCP host
                .WithHttpTransport() // best for remote MCP host
                .WithToolsFromAssembly();    // will auto-discover our tools

            // Register any additional services here
            builder.Services.AddSingleton<ServerContext>();

            WebApplication app = builder.Build();

            app.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to the File MCP Server!");
            });

            app.MapMcp("/mcp"); // This maps the MCP server endpoints

            //INFO: Assigning Host.Services here does not work as application is not yet started.

            await app.RunAsync(); // fire-and-forget
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //StartWebServerV1();
            StartWebServerV2();

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

                endpoints.MapMcp("/mcp"); // This maps the MCP server endpoints
            });
        }
    }
}