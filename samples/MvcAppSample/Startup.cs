using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetAirbrake.AspNetCore;

namespace MvcAppSample
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            // Load configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            this.Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add Airbrake options from configuration
            services.AddAirbrake(options =>
                                    this.Configuration.GetSection("Airbrake").Bind(options));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseAirbrake();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller}/{action}/{id?}");
            });
        }
    }
}