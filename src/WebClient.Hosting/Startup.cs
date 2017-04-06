using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreStack.WebSockets;
using NetCoreStack.WebSockets.ProxyClient;
using System;
using System.Linq;

namespace WebClient.Hosting
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Client WebSocket - Connector orchestration
            services.AddProxyWebSockets(options => {
                options.ConnectorName = $"TestWebApp-{Environment.MachineName}";
                options.WebSocketHostAddress = "localhost:5916"; // Discover.Hosting EndPoint
                options.RegisterInvocator<ProxyWebSocketCommandInvocator>(WebSocketCommands.All);
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();

            // Proxy (Domain App) Client WebSocket
            app.UseProxyWebSockets();

            app.UseMiddleware<AcceptanceMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
